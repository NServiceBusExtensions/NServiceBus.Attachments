using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        await ProcessStreams(context).ConfigureAwait(false);
        await next().ConfigureAwait(false);
    }

    async Task ProcessStreams(IOutgoingLogicalMessageContext context)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<IOutgoingAttachments>(out var attachments))
        {
            return;
        }

        var outgoingAttachments = (OutgoingAttachments) attachments;
        var inner = outgoingAttachments.Inner;
        if (inner.Count == 0)
        {
            return;
        }

        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        if (context.Extensions.TryGet<SqlAttachmentState>(out var state))
        {
            if (state.Transaction != null)
            {
                await ProcessOutgoing(inner, timeToBeReceived, state.Transaction.Connection, state.Transaction, context.MessageId)
                    .ConfigureAwait(false);
                return;
            }

            if (state.Connection != null)
            {
                await ProcessOutgoing(inner, timeToBeReceived, state.Connection, null, context.MessageId)
                    .ConfigureAwait(false);
                return;
            }

            var sqlConnection = await state.GetConnection().ConfigureAwait(false);
            await ProcessOutgoing(inner, timeToBeReceived, sqlConnection, null, context.MessageId)
                .ConfigureAwait(false);
            return;
        }

        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            if (context.TryReadTransaction(out var transaction))
            {
                connection.EnlistTransaction(transaction);
            }

            if (inner.Count == 1)
            {
                var attachment = inner.Single();
                var name = attachment.Key;
                var outgoing = attachment.Value;
                await ProcessAttachment(timeToBeReceived, connection, null, context.MessageId, outgoing, name)
                    .ConfigureAwait(false);
                return;
            }

            using (var sqlTransaction = connection.BeginTransaction())
            {
                await ProcessOutgoing(inner, timeToBeReceived, connection, sqlTransaction, context.MessageId)
                    .ConfigureAwait(false);
                sqlTransaction.Commit();
            }
        }
    }

    Task ProcessOutgoing(Dictionary<string, Outgoing> attachments, TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId)
    {
        return Task.WhenAll(attachments.Select(pair => ProcessAttachment(timeToBeReceived, connection, transaction, messageId, pair.Value, pair.Key)));
    }

    async Task ProcessStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string> metadata)
    {
        using (stream)
        {
            await persister.SaveStream(connection, transaction, messageId, name, expiry, stream, metadata)
                .ConfigureAwait(false);
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId, Outgoing outgoing, string name)
    {
        var outgoingStreamTimeToKeep = outgoing.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            await Process(connection, transaction, messageId, outgoing, name, expiry).ConfigureAwait(false);
        }
        finally
        {
            outgoing.Cleanup?.Invoke();
        }
    }

    async Task Process(SqlConnection connection, SqlTransaction transaction, string messageId, Outgoing outgoing, string name, DateTime expiry)
    {
        if (outgoing.AsyncStreamFactory != null)
        {
            var stream = await outgoing.AsyncStreamFactory().ConfigureAwait(false);
            await ProcessStream(connection, transaction, messageId, name, expiry, stream, outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.StreamFactory != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamFactory(), outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.StreamInstance != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamInstance, outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var bytes = await outgoing.AsyncBytesFactory().ConfigureAwait(false);
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, bytes, outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesFactory != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesFactory(), outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesInstance != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesInstance, outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }
        throw new Exception("No matching way to handle outgoing.");
    }
}