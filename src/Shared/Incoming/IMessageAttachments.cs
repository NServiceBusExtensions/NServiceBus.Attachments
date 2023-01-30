// ReSharper disable PartialTypeWithSinglePart

namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
;

/// <summary>
/// Provides access to read attachments.
/// </summary>
public partial interface IMessageAttachments
{
    /// <summary>
    /// Copy, for the current message, the attachment of <paramref name="name"/> to the <paramref name="target"/> <see cref="Stream"/>.
    /// </summary>
    Task CopyTo(string name, Stream target);

    /// <summary>
    /// Copy, for the current message, the attachment with the default name of <see cref="string.Empty"/> to the <paramref name="target"/> <see cref="Stream"/>.
    /// </summary>
    Task CopyTo(Stream target);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the current message, the attachment of <paramref name="name"/>.
    /// </summary>
    Task ProcessStream(string name, Func<AttachmentStream, Task> action);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    Task ProcessStream(Func<AttachmentStream, Task> action);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, all attachments for the current message.
    /// </summary>
    Task ProcessStreams(Func<AttachmentStream, Task> action);

    /// <summary>
    /// Read all attachment metadata for the current message.
    /// </summary>
    IAsyncEnumerable<AttachmentInfo> GetMetadata();

    /// <summary>
    /// Get a <see cref="byte"/> array, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentBytes> GetBytes();

    /// <summary>
    /// Get a <see cref="MemoryStream"/>, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<MemoryStream> GetMemoryStream();

    /// <summary>
    /// Get a <see cref="byte"/> array, for the current message, the attachment of <paramref name="name"/>.
    /// </summary>
    Task<AttachmentBytes> GetBytes(string name);

    /// <summary>
    /// Get a <see cref="MemoryStream"/>, for the current message, the attachment of <paramref name="name"/>.
    /// </summary>
    Task<MemoryStream> GetMemoryStream(string name);

    /// <summary>
    /// Get a <see cref="string"/>, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentString> GetString(Encoding? encoding = null);

    /// <summary>
    /// Get a <see cref="string"/> array, for the current message, the attachment of <paramref name="name"/>.
    /// </summary>
    Task<AttachmentString> GetString(string name, Encoding? encoding = null);

    /// <summary>
    /// Copy, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/> to the <paramref name="target"/> <see cref="Stream"/>.
    /// </summary>
    Task CopyToForMessage(string messageId, string name, Stream target);

    /// <summary>
    /// Copy, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/> to the <paramref name="target"/> <see cref="Stream"/>.
    /// </summary>
    Task CopyToForMessage(string messageId, Stream target);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
    /// </summary>
    Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action);

    /// <summary>
    /// Process with the delegate <paramref name="action"/>, all attachments for the message with <paramref name="messageId"/>.
    /// </summary>
    Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action);

    /// <summary>
    /// Get a <see cref="byte"/> array, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentBytes> GetBytesForMessage(string messageId);

    /// <summary>
    /// Get a <see cref="MemoryStream"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<MemoryStream> GetMemoryStreamForMessage(string messageId);

    /// <summary>
    /// Get a <see cref="byte"/> array, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentBytes> GetBytesForMessage(string messageId, string name);

    /// <summary>
    /// Get a <see cref="MemoryStream"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name);

    /// <summary>
    /// Get a <see cref="string"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding);

    /// <summary>
    /// Get a <see cref="string"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding);
}