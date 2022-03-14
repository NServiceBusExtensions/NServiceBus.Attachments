namespace NServiceBus.Attachments.FileShare.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(CancellationToken cancellation = default) =>
        GetStream("default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        GetStreamForMessage(messageId, "default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }
}