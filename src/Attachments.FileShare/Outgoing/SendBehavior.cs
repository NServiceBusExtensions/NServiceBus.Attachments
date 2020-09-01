using System;
using System.Collections.Generic;
using System.IO;
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
        var inner = outgoingAttachments.Inner;
        var duplicateIncoming = outgoingAttachments.DuplicateIncomingAttachments;
        if (!outgoingAttachments.HasPendingAttachments)
        {
            return;
        }

        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        foreach (var item in inner)
        {
            var name = item.Key;
            var outgoing = item.Value;
            await ProcessAttachment(timeToBeReceived, context.MessageId, outgoing, name);
        }
        if (duplicateIncoming)
        {
            await persister.Duplicate(context.IncomingMessageId(), context.MessageId);
        }

        foreach (var duplicate in outgoingAttachments.Duplicates)
        {
            if (duplicate.To == null)
            {
                await persister.Duplicate(context.IncomingMessageId(), duplicate.From, context.MessageId);
            }
            else
            {
                await persister.Duplicate(context.IncomingMessageId(), duplicate.From, context.MessageId, duplicate.To);
            }
        }
    }

    async Task ProcessStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata)
    {
        using (stream)
        {
            await persister.SaveStream(messageId, name, expiry, stream, metadata);
        }
    }

    async Task ProcessAttachment(TimeSpan? timeToBeReceived, string messageId, Outgoing outgoing, string name)
    {
        var outgoingStreamTimeToKeep = outgoing.TimeToKeep ?? endpointTimeToKeep;
        var timeToKeep = outgoingStreamTimeToKeep(timeToBeReceived);
        var expiry = DateTime.UtcNow.Add(timeToKeep);
        try
        {
            await Process(messageId, outgoing, name, expiry);
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
            var stream = await outgoing.AsyncStreamFactory();
            await ProcessStream(messageId, name, expiry, stream, outgoing.Metadata);
            return;
        }

        if (outgoing.StreamFactory != null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.StreamInstance != null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var bytes = await outgoing.AsyncBytesFactory();
            await persister.SaveBytes(messageId, name, expiry, bytes, outgoing.Metadata);
            return;
        }

        if (outgoing.BytesFactory != null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.BytesInstance != null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.StringInstance != null)
        {
            await persister.SaveString(messageId, name, expiry, outgoing.StringInstance, outgoing.Encoding, outgoing.Metadata);
            return;
        }

        throw new Exception("No matching way to handle outgoing.");
    }
}