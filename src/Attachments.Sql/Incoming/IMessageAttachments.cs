namespace NServiceBus.Attachments.Sql;

/// <summary>
/// Provides access to read attachments.
/// </summary>
public partial interface IMessageAttachments
{
    /// <summary>
    /// Get a <see cref="Stream" />, for the current message, the attachment with the default name of <see cref="string.Empty" />.
    /// </summary>
    Task<AttachmentStream> GetStream(Cancellation cancel = default);

    /// <summary>
    /// Get a <see cref="Stream" />, for the current message, the attachment of <paramref name="name" />.
    /// </summary>
    Task<AttachmentStream> GetStream(string name, Cancellation cancel = default);

    /// <summary>
    /// Get a <see cref="Stream" />, for the message with <paramref name="messageId" />, the attachment with the default name of <see cref="string.Empty" />.
    /// </summary>
    Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default);

    /// <summary>
    /// Get a <see cref="Stream" />, for the message with <paramref name="messageId" />, the attachment of <paramref name="name" />.
    /// </summary>
    Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the current message, the attachment of <paramref name="name"/>.
    /// </summary>
    Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, all attachments for the current message.
    /// </summary>
    Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
    /// </summary>
    Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, all attachments for the message with <paramref name="messageId"/>.
    /// </summary>
    Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default);
}