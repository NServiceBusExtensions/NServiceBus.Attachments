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

    public Task CopyTo(Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, "default", target, cancel);

    public Task CopyTo(string name, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, name, target, cancel);

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, "default", action, cancel);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, name, action, cancel);

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStreams(messageId, action, cancel);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancel = default) =>
        persister.ReadAllMessageInfo(messageId, cancel);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, "default", encoding, cancel);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, name, encoding, cancel);

    public Task<AttachmentBytes> GetBytes(Cancellation cancel = default) =>
        persister.GetBytes(messageId, "default", cancel);

    public Task<MemoryStream> GetMemoryStream(Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, "default", cancel);

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancel = default) =>
        persister.GetBytes(messageId, name, cancel);

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, name, cancel);

    public Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        persister.GetStream(messageId, "default", cancel);

    public Task<AttachmentStream> GetStream(string name, Cancellation cancel = default) =>
        persister.GetStream(messageId, name, cancel);

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, "default", target, cancel);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, name, target, cancel);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, "default", action, cancel);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, name, action, cancel);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStreams(messageId, action, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetBytes(messageId, "default", cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, "default", cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetBytes(messageId, name, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, name, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, "default", encoding, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, name, encoding, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetStream(messageId, "default", cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetStream(messageId, name, cancel);
}