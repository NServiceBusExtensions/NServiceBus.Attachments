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
    /// <inheritdoc />
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Task> action) =>
        InnerProcessByteArray(name, action);

    /// <inheritdoc />
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Task> action) =>
        ProcessByteArray("default", action);

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(Func<AttachmentBytes, Task> action)
    {
        foreach (var pair in currentAttachments)
        {
            using var attachmentStream = pair.Value.ToAttachmentStream();
            await action(pair.Value.ToAttachmentBytes());
        }
    }
    /// <inheritdoc />
    public virtual async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Task> action)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        await action(attachment.ToAttachmentBytes());
    }

    /// <inheritdoc />
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Task> action) =>
        ProcessByteArrayForMessage("default", messageId, action);

    /// <inheritdoc />
    public virtual async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Task> action)
    {
        foreach (var pair in GetAttachmentsForMessage(messageId))
        {
            var attachment = pair.Value;
            await action(attachment.ToAttachmentBytes());
        }
    }
}