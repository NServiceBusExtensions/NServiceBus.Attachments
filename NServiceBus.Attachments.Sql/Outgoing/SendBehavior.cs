using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    StreamPersister streamPersister;
    GetTimeToKeep endpointTimeToKeep;

    public SendBehavior(Func<Task<SqlConnection>> connectionFactory, StreamPersister streamPersister, GetTimeToKeep timeToKeep)
    {
        this.connectionFactory = connectionFactory;
        this.streamPersister = streamPersister;
        endpointTimeToKeep = timeToKeep;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        await ProcessStreams(context)
            .ConfigureAwait(false);

        await next()
            .ConfigureAwait(false);
    }

    async Task ProcessStreams(IOutgoingLogicalMessageContext context)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<OutgoingAttachments>(out var attachments))
        {
            return;
        }

        var streams = attachments.Streams;
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
        using (var stream = await outgoingStream.Func().ConfigureAwait(false))
        {
            await streamPersister.SaveStream(connection, transaction, messageId, name, DateTime.UtcNow.Add(timeToKeep), stream)
                .ConfigureAwait(false);
        }

        outgoingStream.Cleanup?.Invoke();
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