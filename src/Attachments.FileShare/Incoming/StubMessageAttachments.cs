namespace NServiceBus.Attachments.FileShare.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancellation cancellation = default) =>
        GetStream("default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancellation = default) =>
        GetStreamForMessage(messageId, "default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }
}