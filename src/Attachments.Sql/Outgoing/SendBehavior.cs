﻿using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class SendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<Task<SqlConnection>> connectionFactory;
    IPersister persister;
    GetTimeToKeep endpointTimeToKeep;

    public SendBehavior(Func<Task<SqlConnection>> connectionFactory, IPersister persister, GetTimeToKeep timeToKeep)
    {
        this.connectionFactory = connectionFactory;
        this.persister = persister;
        endpointTimeToKeep = timeToKeep;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        await ProcessStreams(context);
        await next();
    }

    async Task ProcessStreams(IOutgoingLogicalMessageContext context)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<IOutgoingAttachments>(out var attachments))
        {
            return;
        }

        var outgoingAttachments = (OutgoingAttachments) attachments;
        if (!outgoingAttachments.HasPendingAttachments)
        {
            return;
        }

        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        if (context.Extensions.TryGet<SqlAttachmentState>(out var state))
        {
            if (state.Transaction is not null)
            {
                using var connectionFromState = await state.GetConnection();
                connectionFromState.EnlistTransaction(state.Transaction);
                await ProcessOutgoing(timeToBeReceived, connectionFromState, null, context, outgoingAttachments);
                return;
            }

            if (state.SqlTransaction is not null)
            {
                await ProcessOutgoing(timeToBeReceived, state.SqlTransaction.Connection!, state.SqlTransaction, context, outgoingAttachments);
                return;
            }

            if (state.SqlConnection is not null)
            {
                await ProcessOutgoing(timeToBeReceived, state.SqlConnection, null, context, outgoingAttachments);
                return;
            }

            using var connection = await state.GetConnection();
            await ProcessOutgoing(timeToBeReceived, connection, null, context, outgoingAttachments);
            return;
        }

        using var connectionFromFactory = await connectionFactory();
        //TODO: should this be done ?
        if (context.TryReadTransaction(out var transaction))
        {
            connectionFromFactory.EnlistTransaction(transaction);
        }

        using var dbTransaction = connectionFromFactory.BeginTransaction();
        await ProcessOutgoing(timeToBeReceived, connectionFromFactory, dbTransaction, context, outgoingAttachments);
        dbTransaction.Commit();
    }

    async Task ProcessOutgoing(TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction? transaction, IOutgoingLogicalMessageContext context, OutgoingAttachments outgoingAttachments)
    {
        var attachments = new Dictionary<Guid, string>();
        foreach (var (key, value) in outgoingAttachments.Inner)
        {
            var guid = await ProcessAttachment(timeToBeReceived, connection, transaction, context.MessageId, value, key);
            attachments.Add(guid, key);
        }

        if (outgoingAttachments.DuplicateIncomingAttachments)
        {
            if (!context.TryGetIncomingPhysicalMessage(out var incomingMessage))
            {
                throw new("Cannot duplicate incoming when there is no IncomingPhysicalMessage.");
            }

            var names = await persister.Duplicate(incomingMessage.MessageId, connection, transaction, context.MessageId);
            foreach (var name in names)
            {
                attachments.Add(name.Item1, name.Item2);
            }
        }

        foreach (var duplicate in outgoingAttachments.Duplicates)
        {
            var incomingMessageId = context.IncomingMessageId();
            var guid = await persister.Duplicate(incomingMessageId, duplicate.From, connection, transaction, context.MessageId, duplicate.To);
            attachments.Add(guid, duplicate.To);
        }

        Guard.AgainstDuplicateNames(attachments.Values);

        context.Headers.Add("Attachments", string.Join(", ", attachments.Select(x => $"{x.Key}: {x.Value}")));
    }

    async Task<Guid> ProcessStream(SqlConnection connection, SqlTransaction? transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata)
    {
        using (stream)
        {
            return await persister.SaveStream(connection, transaction, messageId, name, expiry, stream, metadata);
        }
    }

    async Task<Guid> ProcessAttachment(TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction? transaction, string messageId, Outgoing outgoing, string name)
    {
        var outgoingStreamTimeToKeep = outgoing.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            return await Process(connection, transaction, messageId, outgoing, name, expiry);
        }
        finally
        {
            outgoing.Cleanup?.Invoke();
        }
    }

    async Task<Guid> Process(SqlConnection connection, SqlTransaction? transaction, string messageId, Outgoing outgoing, string name, DateTime expiry)
    {
        var metadata = outgoing.Metadata;
        if (outgoing.AsyncStreamFactory is not null)
        {
            var stream = await outgoing.AsyncStreamFactory();
            return await ProcessStream(connection, transaction, messageId, name, expiry, stream, metadata);
        }

        if (outgoing.StreamFactory is not null)
        {
            return await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamFactory(), metadata);
        }

        if (outgoing.StreamInstance is not null)
        {
            return await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamInstance, metadata);
        }

        if (outgoing.AsyncBytesFactory is not null)
        {
            var bytes = await outgoing.AsyncBytesFactory();
            return await persister.SaveBytes(connection, transaction, messageId, name, expiry, bytes, metadata);
        }

        if (outgoing.BytesFactory is not null)
        {
            return await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesFactory(), metadata);
        }

        if (outgoing.BytesInstance is not null)
        {
            return await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesInstance, metadata);
        }

        if (outgoing.StringInstance is not null)
        {
            return await persister.SaveString(connection, transaction, messageId, name, expiry, outgoing.StringInstance, outgoing.Encoding, metadata);
        }

        throw new("No matching way to handle outgoing.");
    }
}