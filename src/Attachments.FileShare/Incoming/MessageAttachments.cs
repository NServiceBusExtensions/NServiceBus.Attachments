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

    public Task CopyTo(Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, "default", target, cancel);

    public Task CopyTo(string name, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, name, target, cancel);

    public Task ProcessStream(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, "default", action, cancel);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, name, action, cancel);

    public Task ProcessStreams(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStreams(messageId, action, cancel);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancel cancel = default) =>
        persister.ReadAllMessageInfo(messageId, cancel);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, "default", encoding, cancel);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, name, encoding, cancel);

    public Task<AttachmentBytes> GetBytes(Cancel cancel = default) =>
        persister.GetBytes(messageId, "default", cancel);

    public Task<MemoryStream> GetMemoryStream(Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, "default", cancel);

    public Task<AttachmentBytes> GetBytes(string name, Cancel cancel = default) =>
        persister.GetBytes(messageId, name, cancel);

    public Task<MemoryStream> GetMemoryStream(string name, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, name, cancel);

    public Task<AttachmentStream> GetStream(Cancel cancel = default) =>
        persister.GetStream(messageId, "default", cancel);

    public Task<AttachmentStream> GetStream(string name, Cancel cancel = default) =>
        persister.GetStream(messageId, name, cancel);

    public Task CopyToForMessage(string messageId, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, "default", target, cancel);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, name, target, cancel);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, "default", action, cancel);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, name, action, cancel);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStreams(messageId, action, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancel cancel = default) =>
        persister.GetBytes(messageId, "default", cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, "default", cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetBytes(messageId, name, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, name, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, "default", encoding, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, name, encoding, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancel cancel = default) =>
        persister.GetStream(messageId, "default", cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetStream(messageId, name, cancel);
}