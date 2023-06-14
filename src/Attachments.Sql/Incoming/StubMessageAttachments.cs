namespace NServiceBus.Attachments.Sql.Testing;

public partial class StubMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancellation cancellation = default) =>
        GetStream("default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        var attachmentStream = attachment.ToAttachmentStream();
        return Task.FromResult(attachmentStream);
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
    /// <inheritdoc />
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        InnerProcessByteArray(name, action, cancellation);

    /// <inheritdoc />
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        ProcessByteArray("default", action, cancellation);

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        foreach (var pair in currentAttachments)
        {
            using var attachmentStream = pair.Value.ToAttachmentStream();
            await action(pair.Value.ToAttachmentBytes(), cancellation);
        }
    }
    /// <inheritdoc />
    public virtual async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        await action(attachment.ToAttachmentBytes(), cancellation);
    }

    /// <inheritdoc />
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        ProcessByteArrayForMessage("default", messageId, action, cancellation);

    /// <inheritdoc />
    public virtual async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        foreach (var pair in GetAttachmentsForMessage(messageId))
        {
            await action(pair.Value.ToAttachmentBytes(), cancellation);
        }
    }
}