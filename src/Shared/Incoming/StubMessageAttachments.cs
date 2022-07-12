namespace NServiceBus.Attachments
#if FileShare
.FileShare.Testing
#endif
#if Sql
.Sql.Testing
#endif
;

/// <summary>
/// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
/// All members are stubbed out.
/// </summary>
/// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
public partial class StubMessageAttachments :
    IMessageAttachments
{
    string messageId;

    /// <summary>
    /// Instantiate a new instance of <see cref="StubMessageAttachments"/>.
    /// </summary>
    public StubMessageAttachments() =>
        messageId = Guid.NewGuid().ToString();

    /// <summary>
    /// Instantiate a new instance of <see cref="StubMessageAttachments"/>.
    /// </summary>
    public StubMessageAttachments(string messageId)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        this.messageId = messageId;
    }

    /// <inheritdoc />
    public virtual Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        CopyCurrentMessageAttachmentToStream(name, target, null);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task CopyTo(Stream target, CancellationToken cancellation = default) =>
        CopyTo("default", target, cancellation);

    /// <inheritdoc />
    public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        InnerProcessStream(name, action);

    /// <inheritdoc />
    public virtual Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        ProcessStream("default", action, cancellation);

    /// <inheritdoc />
    public virtual async Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        foreach (var pair in currentAttachments)
        {
            await using var attachmentStream = pair.Value.ToAttachmentStream();
            await action(attachmentStream);
        }
    }

    /// <inheritdoc />
    public virtual Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default) =>
        GetString("default", encoding, cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(attachment.ToAttachmentString(encoding));
    }

    /// <inheritdoc />
    public virtual Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default) =>
        GetBytes("default", cancellation);

    /// <inheritdoc />
    public virtual Task<MemoryStream> GetMemoryStream(CancellationToken cancellation = default) =>
        GetMemoryStream("default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(attachment.ToAttachmentBytes());
    }

    /// <inheritdoc />
    public virtual Task<MemoryStream> GetMemoryStream(string name, CancellationToken cancellation = default)
    {
        var attachment = GetCurrentMessageAttachment(name);
        return Task.FromResult(new MemoryStream(attachment.ToAttachmentBytes()));
    }

    /// <inheritdoc />
    public virtual Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        using var writer = BuildWriter(target, null);
        writer.Write(attachment.Bytes);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default) =>
        CopyToForMessage(messageId, "default", target, cancellation);

    /// <inheritdoc />
    public virtual async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        await using var attachmentStream = attachment.ToAttachmentStream();
        await action(attachmentStream);
    }

    /// <inheritdoc />
    public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        ProcessStreamForMessage("default", messageId, action, cancellation);

    /// <inheritdoc />
    public virtual async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        foreach (var pair in GetAttachmentsForMessage(messageId))
        {
            var attachment = pair.Value;
            await using var attachmentStream = attachment.ToAttachmentStream();
            await action(attachmentStream);
        }
    }

    /// <inheritdoc />
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default) =>
        GetBytesForMessage(messageId, "default", cancellation);

    /// <inheritdoc />
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        GetMemoryStreamForMessage(messageId, "default", cancellation);

    /// <inheritdoc />
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default) =>
        GetStringForMessage(messageId, "default", encoding, cancellation);

    /// <inheritdoc />
    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default)
    {
        var infos = currentAttachments.Select(_ => new AttachmentInfo(messageId, _.Key, _.Value.Expiry, _.Value.Metadata));
        return new AsyncEnumerable<AttachmentInfo>(infos);
    }

    /// <inheritdoc />
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentBytes());
    }

    /// <inheritdoc />
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(new MemoryStream(attachment.ToAttachmentBytes()));
    }

    /// <inheritdoc />
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        var attachment = GetAttachmentForMessage(messageId, name);
        return Task.FromResult(attachment.ToAttachmentString(encoding));
    }

    void CopyCurrentMessageAttachmentToStream(string name, Stream target, Encoding? encoding)
    {
        var bytes = GetCurrentMessageBytes(name);

        using var writer = BuildWriter(target, encoding);
        writer.Write(bytes);
    }

    byte[] GetCurrentMessageBytes(string name)
    {
        if (currentAttachments.TryGetValue(name, out var attachment))
        {
            return attachment.Bytes;
        }

        throw new($"Cant find an attachment: {name}");
    }

    MockAttachment GetAttachmentForMessage(string messageId, string name)
    {
        var attachmentsForMessage = GetAttachmentsForMessage(messageId);
        if (attachmentsForMessage.TryGetValue(name, out var attachment))
        {
            return attachment;
        }

        throw new($"Cant find an attachment: {name}");
    }

    Dictionary<string, MockAttachment> GetAttachmentsForMessage(string messageId)
    {
        if (attachments.TryGetValue(messageId, out var attachmentsForMessage))
        {
            return attachmentsForMessage;
        }

        throw new($"Cant find an attachment: {messageId}");
    }

    MockAttachment GetCurrentMessageAttachment(string name)
    {
        if (currentAttachments.TryGetValue(name, out var attachment))
        {
            return attachment;
        }

        throw new($"Cant find an attachment: {name}");
    }

    static BinaryWriter BuildWriter(Stream target, Encoding? encoding) =>
        new(target, encoding.Default(), leaveOpen: true);

    Task InnerProcessStream(string name, Func<AttachmentStream, Task> action)
    {
        var attachment = GetCurrentMessageAttachment(name);
        using var attachmentStream = attachment.ToAttachmentStream();
        return action(attachmentStream);
    }
}