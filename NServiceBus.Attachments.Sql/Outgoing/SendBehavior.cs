using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class SendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<CancellationToken, Task<SqlConnection>> connectionFactory;
    Persister persister;
    GetTimeToKeep endpointTimeToKeep;
    CancellationToken endpointCancellation;

    public SendBehavior(Func<CancellationToken, Task<SqlConnection>> connectionFactory, Persister persister, GetTimeToKeep timeToKeep, CancellationToken endpointCancellation)
    {
        this.connectionFactory = connectionFactory;
        this.persister = persister;
        endpointTimeToKeep = timeToKeep;
        this.endpointCancellation = endpointCancellation;
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

        var outgoingAttachments = (OutgoingAttachments)attachments;
        var streams = outgoingAttachments.Streams;
        if (!streams.Any())
        {
            return;
        }

        var currentToken = outgoingAttachments.Cancellation.Or(endpointCancellation);
        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        using (var connection = await connectionFactory(currentToken).ConfigureAwait(false))
        {
            if (context.TryReadTransaction(out var transaction))
            {
                connection.EnlistTransaction(transaction);
            }

            if (streams.Count == 1)
            {
                await ProcessOutgoing(streams, timeToBeReceived, connection, null, context.MessageId)
                    .ConfigureAwait(false);
                return;
            }

            using (var sqlTransaction = connection.BeginTransaction())
            {
                await ProcessOutgoing(streams, timeToBeReceived, connection, sqlTransaction, context.MessageId)
                    .ConfigureAwait(false);
                sqlTransaction.Commit();
            }
        }
    }

    async Task ProcessOutgoing(Dictionary<string, Outgoing> attachments, TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId)
    {
        foreach (var attachment in attachments)
        {
            var name = attachment.Key;
            var outgoing = attachment.Value;
            await ProcessAttachment(timeToBeReceived, connection, transaction, messageId, outgoing, name)
                .ConfigureAwait(false);
        }
    }

    async Task ProcessStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream, CancellationToken cancellation)
    {
        using (stream)
        {
            await persister.SaveStream(connection, transaction, messageId, name, expiry, stream, cancellation)
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
            await Process(connection, transaction, messageId, outgoing, name, expiry);
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
            await ProcessStream(connection, transaction, messageId, name, expiry, stream, outgoing.Cancellation).ConfigureAwait(false);
            return;
        }

        if (outgoing.StreamFactory != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamFactory(), outgoing.Cancellation);
            return;
        }

        if (outgoing.StreamInstance != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoing.StreamInstance, outgoing.Cancellation).ConfigureAwait(false);
            return;
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var bytes = await outgoing.AsyncBytesFactory().ConfigureAwait(false);
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, bytes, outgoing.Cancellation)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesFactory != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesFactory(), outgoing.Cancellation)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesInstance != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoing.BytesInstance, outgoing.Cancellation)
                .ConfigureAwait(false);
            return;
        }
        throw new Exception("No matching way to handle outgoing.");
    }
}