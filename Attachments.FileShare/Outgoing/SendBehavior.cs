using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Pipeline;

class SendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    IPersister persister;
    GetTimeToKeep endpointTimeToKeep;

    public SendBehavior(IPersister persister, GetTimeToKeep timeToKeep)
    {
        this.persister = persister;
        endpointTimeToKeep = timeToKeep;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        await ProcessStreams(context).ConfigureAwait(false);
        await next().ConfigureAwait(false);
    }

    Task ProcessStreams(IOutgoingLogicalMessageContext context)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<IOutgoingAttachments>(out var attachments))
        {
            return Task.CompletedTask;
        }

        var outgoingAttachments = (OutgoingAttachments) attachments;
        var inner = outgoingAttachments.Inner;
        if (!inner.Any())
        {
            return Task.CompletedTask;
        }

        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        return ProcessOutgoing(inner, timeToBeReceived, context.MessageId);
    }

    Task ProcessOutgoing(Dictionary<string, Outgoing> attachments, TimeSpan? timeToBeReceived, string messageId)
    {
        return Task.WhenAll(
            attachments.Select(pair =>
            {
                var name = pair.Key;
                var outgoing = pair.Value;
                return ProcessAttachment(timeToBeReceived, messageId, outgoing, name);
            }));
    }

    async Task ProcessStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string> metadata)
    {
        using (stream)
        {
            await persister.SaveStream(messageId, name, expiry, stream, metadata)
                .ConfigureAwait(false);
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, string messageId, Outgoing outgoing, string name)
    {
        var outgoingStreamTimeToKeep = outgoing.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            await Process(messageId, outgoing, name, expiry).ConfigureAwait(false);
        }
        finally
        {
            outgoing.Cleanup?.Invoke();
        }
    }

    async Task Process(string messageId, Outgoing outgoing, string name, DateTime expiry)
    {
        if (outgoing.AsyncStreamFactory != null)
        {
            var stream = await outgoing.AsyncStreamFactory().ConfigureAwait(false);
            await ProcessStream(messageId, name, expiry, stream, outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.StreamFactory != null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamFactory(), outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.StreamInstance != null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamInstance, outgoing.Metadata).ConfigureAwait(false);
            return;
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var bytes = await outgoing.AsyncBytesFactory().ConfigureAwait(false);
            await persister.SaveBytes(messageId, name, expiry, bytes, outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesFactory != null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesFactory(), outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }

        if (outgoing.BytesInstance != null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesInstance, outgoing.Metadata)
                .ConfigureAwait(false);
            return;
        }
        throw new Exception("No matching way to handle outgoing.");
    }
}