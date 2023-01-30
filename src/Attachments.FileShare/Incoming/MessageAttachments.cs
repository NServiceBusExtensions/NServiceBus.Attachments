using NServiceBus.Attachments.FileShare;

class MessageAttachments :
    IMessageAttachments
{
    string messageId;
    IPersister persister;
    CancellationToken cancellation;

    internal MessageAttachments(string messageId, IPersister persister, CancellationToken cancellation)
    {
        this.messageId = messageId;
        this.persister = persister;
        this.cancellation = cancellation;
    }

    public Task CopyTo(Stream target) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyTo(string name, Stream target) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        persister.ReadAllMessageInfo(messageId, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentBytes> GetBytes() =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStream() =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytes(string name) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentStream> GetStream() =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStream(string name) =>
        persister.GetStream(messageId, name, cancellation);

    public Task CopyToForMessage(string messageId, Stream target) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        persister.GetStream(messageId, name, cancellation);
}