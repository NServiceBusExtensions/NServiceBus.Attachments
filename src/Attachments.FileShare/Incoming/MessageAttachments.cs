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

    public Task CopyTo(Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default) =>
        persister.ReadAllMessageInfo(messageId, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStream(CancellationToken cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name, CancellationToken cancellation = default) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentStream> GetStream(CancellationToken cancellation = default) =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, name, cancellation);

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, name, cancellation);
}