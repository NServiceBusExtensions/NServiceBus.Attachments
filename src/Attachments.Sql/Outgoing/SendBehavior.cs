using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class SendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<Task<DbConnection>> connectionFactory;
    IPersister persister;
    GetTimeToKeep endpointTimeToKeep;

    public SendBehavior(Func<Task<DbConnection>> connectionFactory, IPersister persister, GetTimeToKeep timeToKeep)
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
            if (state.Transaction != null)
            {
                await using (var connection = await state.GetConnection())
                {
                    connection.EnlistTransaction(state.Transaction);
                    await ProcessOutgoing(timeToBeReceived, connection, null, context, outgoingAttachments);
                }

                return;
            }

            if (state.DbTransaction != null)
            {
                await ProcessOutgoing(timeToBeReceived, state.DbTransaction.Connection, state.DbTransaction, context, outgoingAttachments);
                return;
            }

            if (state.DbConnection != null)
            {
                await ProcessOutgoing(timeToBeReceived, state.DbConnection, null, context, outgoingAttachments);
                return;
            }

            await using (var connection = await state.GetConnection())
            {
                await ProcessOutgoing(timeToBeReceived, connection, null, context, outgoingAttachments);
            }

            return;
        }

        await using (var connection = await connectionFactory())
        {
            //TODO: should this be done ?
            if (context.TryReadTransaction(out var transaction))
            {
                connection.EnlistTransaction(transaction);
            }

            await using var dbTransaction = connection.BeginTransaction();
            await ProcessOutgoing(timeToBeReceived, connection, dbTransaction, context, outgoingAttachments);
            dbTransaction.Commit();
        }
    }

    Task ProcessOutgoing(TimeSpan? timeToBeReceived, DbConnection connection, DbTransaction? transaction, IOutgoingLogicalMessageContext context, OutgoingAttachments outgoingAttachments)
    {
        var tasks = outgoingAttachments.Inner
            .Select(pair => ProcessAttachment(timeToBeReceived, connection, transaction, context.MessageId, pair.Value, pair.Key))
            .ToList();
        if (outgoingAttachments.DuplicateIncomingAttachments)
        {
            if (!context.TryGetIncomingPhysicalMessage(out var incomingMessage))
            {
                throw new Exception("Cannot duplicate incoming when there is no IncomingPhysicalMessage.");
            }

            tasks.Add(persister.Duplicate(incomingMessage.MessageId, connection, transaction, context.MessageId));
        }

        foreach (var duplicate in outgoingAttachments.Duplicates)
        {
            if (duplicate.To == null)
            {
                tasks.Add(persister.Duplicate(context.IncomingMessageId(), duplicate.From, connection, transaction, context.MessageId));
            }
            else
            {
                tasks.Add(persister.Duplicate(context.IncomingMessageId(), duplicate.From, connection, transaction, context.MessageId, duplicate.To));
            }
        }

        return Task.WhenAll(tasks);
    }

    async Task ProcessStream(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata)
    {
        await using (stream)
        {
            await persister.SaveStream(connection, transaction, messageId, name, expiry, stream, metadata);
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, DbConnection connection, DbTransaction? transaction, string messageId, Outgoing outgoing, string name)
    {
        var outgoingStreamTimeToKeep = outgoing.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            await Process(connection, transaction, messageId, outgoing, name, expiry);
        }
        finally
        {
            outgoing.Cleanup?.Invoke();
        }
    }

    async Task Process(DbConnection connection, DbTransaction? transaction, string messageId, Outgoing outgoing, string name, DateTime expiry)
    {
        if (outgoing.AsyncStreamFactory != null)
        {
            var stream = await outgoing.AsyncStreamFactory();
            await ProcessStream(connection, transaction, messageId, name, expiry, stream, outgoing.Metadata);
            return;
        }

        if (outgoing.StreamFactory != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.StreamInstance != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var bytes = await outgoing.AsyncBytesFactory();
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, bytes, outgoing.Metadata);
            return;
        }

        if (outgoing.BytesFactory != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.BytesInstance != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.StringInstance != null)
        {
            await persister.SaveString(connection, transaction, messageId, name, expiry, outgoing.StringInstance, outgoing.Metadata);
            return;
        }

        throw new Exception("No matching way to handle outgoing.");
    }
}