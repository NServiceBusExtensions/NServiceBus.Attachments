namespace NServiceBus.Attachments.Sql.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        GetStream("default", cancel);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancellation cancel = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        var attachmentStream = attachment.ToAttachmentStream();
        return Task.FromResult(attachmentStream);
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
    /// <inheritdoc />
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        InnerProcessByteArray(name, action, cancel);

    /// <inheritdoc />
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        ProcessByteArray("default", action, cancel);

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        foreach (var pair in currentAttachments)
        {
            using var attachmentStream = pair.Value.ToAttachmentStream();
            await action(pair.Value.ToAttachmentBytes(), cancel);
        }
    }
    /// <inheritdoc />
    public virtual async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        await action(attachment.ToAttachmentBytes(), cancel);
    }

    /// <inheritdoc />
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        ProcessByteArrayForMessage("default", messageId, action, cancel);

    /// <inheritdoc />
    public virtual async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        foreach (var pair in GetAttachmentsForMessage(messageId))
        {
            await action(pair.Value.ToAttachmentBytes(), cancel);
        }
    }
}