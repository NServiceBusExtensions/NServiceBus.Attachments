namespace NServiceBus.Attachments.Sql.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream() =>
        GetStream("default");

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name)
    {
        var attachment = GetCurrentMessageAttachment(name);
        var attachmentStream = attachment.ToAttachmentStream();
        return Task.FromResult(attachmentStream);
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        GetStreamForMessage(messageId, "default");

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }
}