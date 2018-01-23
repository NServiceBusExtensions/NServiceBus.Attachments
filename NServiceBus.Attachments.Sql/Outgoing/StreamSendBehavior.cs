using System;
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

    public StreamSendBehavior(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister)
    {
        this.connectionBuilder = connectionBuilder;
        this.streamPersister = streamPersister;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<OutgoingAttachments>(out var attachments))
        {
            return;
        }

        var streams = attachments.Streams;
        if (streams.Any())
        {
            var timeToBeReceived = GetTimeToBeReceivedFromConstraint(extensions);

            using (var connection = connectionBuilder())
            {
                await connection.OpenAsync();
                var messageId = context.MessageId;
                foreach (var attachment in streams)
                {
                    var name = attachment.Key;
                    var outgoingStream = attachment.Value;
                    var timeToKeep = outgoingStream.TimeToKeep(timeToBeReceived);
                    var stream = outgoingStream.Func();
                    await streamPersister.SaveStream(connection, messageId, name, DateTime.UtcNow.Add(timeToKeep), stream);
                }
            }
        }

        await next()
            .ConfigureAwait(false);
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