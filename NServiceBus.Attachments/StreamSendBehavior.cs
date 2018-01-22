using System;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.DeliveryConstraints;
using NServiceBus.Extensibility;
using NServiceBus.Performance.TimeToBeReceived;
using NServiceBus.Pipeline;

class StreamSendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<IOutgoingLogicalMessageContext, ConnectionAndTransaction> connectionBuilder;

    public StreamSendBehavior(Func<IOutgoingLogicalMessageContext, ConnectionAndTransaction> connectionBuilder)
    {
        this.connectionBuilder = connectionBuilder;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<OutgoingAttachments>(out var attachments))
        {
            return;
        }

        var timeToBeReceived = GetTimeToBeReceivedFromConstraint(extensions);

        var connectionAndTransaction = connectionBuilder(context);
        var messageId = context.MessageId;
        foreach (var attachmentsStream in attachments.Streams)
        {
            var name = attachmentsStream.Key;
            var outgoingStream = attachmentsStream.Value;
            var timeToKeep = outgoingStream.TimeToKeep(timeToBeReceived);
            var stream = outgoingStream.Func();
            await StreamPersister.SaveStream(connectionAndTransaction.Connection, connectionAndTransaction.Transaction, messageId, name, timeToKeep, stream);
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