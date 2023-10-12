namespace NServiceBus.Attachments.Sql.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancel cancel = default) =>
        GetStream("default", cancel);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancel cancel = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        var attachmentStream = attachment.ToAttachmentStream();
        return Task.FromResult(attachmentStream);
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, Cancel cancel = default) =>
        GetStreamForMessage(messageId, "default", cancel);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancel cancel = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentStream());
    }
    /// <inheritdoc />
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        InnerProcessByteArray(name, action, cancel);

    /// <inheritdoc />
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        ProcessByteArray("default", action, cancel);

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default)
    {
        foreach (var pair in currentAttachments)
        {
            using var attachmentStream = pair.Value.ToAttachmentStream();
            await action(pair.Value.ToAttachmentBytes(), cancel);
        }
    }
    /// <inheritdoc />
    public virtual Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return action(attachment.ToAttachmentBytes(), cancel);
    }

    /// <inheritdoc />
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        ProcessByteArrayForMessage("default", messageId, action, cancel);

    /// <inheritdoc />
    public virtual async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default)
    {
        foreach (var pair in GetAttachmentsForMessage(messageId))
        {
            await action(pair.Value.ToAttachmentBytes(), cancel);
        }
    }
}