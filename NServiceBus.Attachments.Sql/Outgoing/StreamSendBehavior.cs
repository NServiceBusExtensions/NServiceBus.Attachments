﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.DeliveryConstraints;
using NServiceBus.Extensibility;
using NServiceBus.Performance.TimeToBeReceived;
using NServiceBus.Pipeline;

class StreamSendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<SqlConnection> connectionBuilder;
    StreamPersister streamPersister;
    GetTimeToKeep endpointTimeToKeep;

    public StreamSendBehavior(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister, GetTimeToKeep timeToKeep)
    {
        this.connectionBuilder = connectionBuilder;
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

        using (var connection = connectionBuilder())
        {
            await connection.OpenAsync();
            var messageId = context.MessageId;

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var attachment in streams)
                {
                    var name = attachment.Key;
                    var outgoingStream = attachment.Value;
                    await ProcessAttachment(timeToBeReceived, connection, transaction, messageId, outgoingStream, name)
                        .ConfigureAwait(false);
                }

                transaction.Commit();
            }
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, SqlConnection connection, SqlTransaction transaction, string messageId, OutgoingStream outgoingStream, string name)
    {
        var outgoingStreamTimeToKeep = outgoingStream.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var stream = await outgoingStream.Func().ConfigureAwait(false);
        await streamPersister.SaveStream(connection, transaction, messageId, name, DateTime.UtcNow.Add(timeToKeep), stream)
            .ConfigureAwait(false);
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
