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

    public Task CopyTo(Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyTo(string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancellation = default) =>
        persister.ReadAllMessageInfo(messageId, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentBytes> GetBytes(Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStream(Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentStream> GetStream(Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, cancellation);

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, cancellation);
}