using NServiceBus.Attachments.FileShare;

class MessageAttachments :
    IMessageAttachments
{
    string messageId;
    IPersister persister;

    internal MessageAttachments(string messageId, IPersister persister)
    {
        this.messageId = messageId;
        this.persister = persister;
    }

    public Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, "default", target, cancellation);
    }

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, name, target, cancellation);
    }

    public Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, "default", action, cancellation);
    }

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, name, action, cancellation);
    }

    public Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStreams(messageId, action, cancellation);
    }

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default)
    {
        return persister.ReadAllMessageInfo(messageId, cancellation);
    }

    public Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, "default", encoding, cancellation);
    }

    public Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, name, encoding, cancellation);
    }

    public Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, "default", cancellation);
    }

    public Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, name, cancellation);
    }

    public Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, "default",cancellation);
    }

    public Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, name,cancellation);
    }

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, "default", target, cancellation);
    }

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, name, target, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, "default", action, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, name, action, cancellation);
    }

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStreams(messageId, action, cancellation);
    }

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, "default", cancellation);
    }

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, name, cancellation);
    }

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, "default", encoding, cancellation);
    }

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, name, encoding, cancellation);
    }

    public Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, "default",cancellation);
    }

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, name,cancellation);
    }
}