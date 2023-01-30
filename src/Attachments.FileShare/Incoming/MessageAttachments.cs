using NServiceBus.Attachments.FileShare;

class MessageAttachments :
    IMessageAttachments
{
    string messageId;
    IPersister persister;
    public CancellationToken Cancellation { get; }

    internal MessageAttachments(string messageId, IPersister persister, CancellationToken cancellation)
    {
        this.messageId = messageId;
        this.persister = persister;
        Cancellation = cancellation;
    }

    public Task CopyTo(Stream target) =>
        persister.CopyTo(messageId, "default", target, Cancellation);

    public Task CopyTo(string name, Stream target) =>
        persister.CopyTo(messageId, name, target, Cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", action, Cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, action, Cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, action, Cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        persister.ReadAllMessageInfo(messageId, Cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding) =>
        persister.GetString(messageId, "default", encoding, Cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        persister.GetString(messageId, name, encoding, Cancellation);

    public Task<AttachmentBytes> GetBytes() =>
        persister.GetBytes(messageId, "default", Cancellation);

    public Task<MemoryStream> GetMemoryStream() =>
        persister.GetMemoryStream(messageId, "default", Cancellation);

    public Task<AttachmentBytes> GetBytes(string name) =>
        persister.GetBytes(messageId, name, Cancellation);

    public Task<MemoryStream> GetMemoryStream(string name) =>
        persister.GetMemoryStream(messageId, name, Cancellation);

    public Task<AttachmentStream> GetStream() =>
        persister.GetStream(messageId, "default", Cancellation);

    public Task<AttachmentStream> GetStream(string name) =>
        persister.GetStream(messageId, name, Cancellation);

    public Task CopyToForMessage(string messageId, Stream target) =>
        persister.CopyTo(messageId, "default", target, Cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target) =>
        persister.CopyTo(messageId, name, target, Cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", action, Cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, action, Cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, action, Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        persister.GetBytes(messageId, "default", Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        persister.GetMemoryStream(messageId, "default", Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        persister.GetBytes(messageId, name, Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        persister.GetMemoryStream(messageId, name, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        persister.GetString(messageId, "default", encoding, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        persister.GetString(messageId, name, encoding, Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        persister.GetStream(messageId, "default", Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        persister.GetStream(messageId, name, Cancellation);
}