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
        await ProcessOutgoing(context);
        await next();
    }

    async Task ProcessOutgoing(IOutgoingLogicalMessageContext context)
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

        var attachmentNames = new List<string>();

        var timeToBeReceived = extensions.GetTimeToBeReceivedFromConstraint();

        foreach (var (name, value) in outgoingAttachments.Inner)
        {
            await ProcessAttachment(timeToBeReceived, context.MessageId, value, name);
            attachmentNames.Add(name);
        }

        var incomingMessageId = context.IncomingMessageId();
        if (outgoingAttachments.DuplicateIncomingAttachments)
        {
            var names = await persister.Duplicate(incomingMessageId, context.MessageId);
            attachmentNames.AddRange(names);
        }

        foreach (var duplicate in outgoingAttachments.Duplicates)
        {
            attachmentNames.Add(duplicate.To);
            await persister.Duplicate(incomingMessageId, duplicate.From, context.MessageId, duplicate.To);
        }

        Guard.AgainstDuplicateNames(attachmentNames);

        context.Headers.Add("Attachments", string.Join(", ", attachmentNames));
    }

    async Task ProcessStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata)
    {
        await using (stream)
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
        if (outgoing.AsyncStreamFactory is not null)
        {
            var stream = await outgoing.AsyncStreamFactory();
            await ProcessStream(messageId, name, expiry, stream, outgoing.Metadata);
            return;
        }

        if (outgoing.StreamFactory is not null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.StreamInstance is not null)
        {
            await ProcessStream(messageId, name, expiry, outgoing.StreamInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.AsyncBytesFactory is not null)
        {
            var bytes = await outgoing.AsyncBytesFactory();
            await persister.SaveBytes(messageId, name, expiry, bytes, outgoing.Metadata);
            return;
        }

        if (outgoing.BytesFactory is not null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesFactory(), outgoing.Metadata);
            return;
        }

        if (outgoing.BytesInstance is not null)
        {
            await persister.SaveBytes(messageId, name, expiry, outgoing.BytesInstance, outgoing.Metadata);
            return;
        }

        if (outgoing.StringInstance is not null)
        {
            await persister.SaveString(messageId, name, expiry, outgoing.StringInstance, outgoing.Encoding, outgoing.Metadata);
            return;
        }

        throw new("No matching way to handle outgoing.");
    }
}