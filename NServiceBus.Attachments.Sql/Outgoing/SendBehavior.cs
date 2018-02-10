using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.DeliveryConstraints;
using NServiceBus.Extensibility;
using NServiceBus.Performance.TimeToBeReceived;
using NServiceBus.Pipeline;

class SendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<Task<SqlConnection>> connectionFactory;
    StreamPersister persister;
    GetTimeToKeep endpointTimeToKeep;

    public SendBehavior(Func<Task<SqlConnection>> connectionFactory, StreamPersister persister, GetTimeToKeep timeToKeep)
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

        var streams = ((OutgoingAttachments)attachments).Streams;
        if (!streams.Any())
        {
            return;
        }

        var timeToBeReceived = GetTimeToBeReceivedFromConstraint(extensions);

        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            if (context.TryReadTransaction(out var transaction))
            {
                connection.EnlistTransaction(transaction);
            }

            if (streams.Count == 1)
            {
                await ProcessStreams(streams, timeToBeReceived, connection, null, context.MessageId)
                    .ConfigureAwait(false);
                return;
            }

            using (var sqlTransaction = connection.BeginTransaction())
            {
                await ProcessStreams(streams, timeToBeReceived, connection, sqlTransaction, context.MessageId)
                    .ConfigureAwait(false);
                sqlTransaction.Commit();
            }
        }
    }

    async Task ProcessStreams(Dictionary<string, OutgoingStream> streams, TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId)
    {
        foreach (var attachment in streams)
        {
            var name = attachment.Key;
            var outgoingStream = attachment.Value;
            await ProcessAttachment(timeToBeReceived, connection, transaction, messageId, outgoingStream, name)
                .ConfigureAwait(false);
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId, OutgoingStream outgoingStream, string name)
    {
        var outgoingStreamTimeToKeep = outgoingStream.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            await Process(connection, transaction, messageId, outgoingStream, name, expiry);
        }
        finally
        {
            outgoingStream.Cleanup?.Invoke();
        }
    }

    async Task Process(SqlConnection connection, SqlTransaction transaction, string messageId, OutgoingStream outgoingStream, string name, DateTime expiry)
    {
        if (outgoingStream.AsyncStreamFactory != null)
        {
            var stream = await outgoingStream.AsyncStreamFactory().ConfigureAwait(false);
            await ProcessStream(connection, transaction, messageId, name, expiry, stream).ConfigureAwait(false);
            return;
        }

        if (outgoingStream.StreamFactory != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoingStream.StreamFactory());
            return;
        }

        if (outgoingStream.StreamInstance != null)
        {
            await ProcessStream(connection, transaction, messageId, name, expiry, outgoingStream.StreamInstance).ConfigureAwait(false);
            return;
        }
        if (outgoingStream.AsyncBytesFactory != null)
        {
            var bytes = await outgoingStream.AsyncBytesFactory().ConfigureAwait(false);
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, bytes)
                .ConfigureAwait(false);
            return;
        }

        if (outgoingStream.BytesFactory != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoingStream.BytesFactory())
                .ConfigureAwait(false);
            return;
        }

        if (outgoingStream.BytesInstance != null)
        {
            await persister.SaveBytes(connection, transaction, messageId, name, expiry, outgoingStream.BytesInstance)
                .ConfigureAwait(false);
            return;
        }
        throw new Exception("No matching way to handle outgoingStream.");
    }

    async Task ProcessStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream)
    {
        using (stream)
        {
            await persister.SaveStream(connection, transaction, messageId, name, expiry, stream)
                .ConfigureAwait(false);
        }
    }

    static TimeSpan? GetTimeToBeReceivedFromConstraint(ContextBag extensions)
    {
        if (extensions.TryGetDeliveryConstraint<DiscardIfNotReceivedBefore>(out var constraint))
        {
            return constraint.MaxTime;
        }

        return null;
    }
}