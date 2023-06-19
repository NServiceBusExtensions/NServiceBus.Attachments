namespace NServiceBus.Attachments.FileShare.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        GetStream("default", cancel);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancellation cancel = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default) =>
        GetStreamForMessage(messageId, "default", cancel);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }
}