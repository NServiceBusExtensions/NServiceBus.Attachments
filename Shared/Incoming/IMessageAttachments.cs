using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable PartialTypeWithSinglePart

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#endif
#if Sql
.Sql
#endif
{
    /// <summary>
    /// Provides access to read attachments.
    /// </summary>
    public partial interface IMessageAttachments
    {
        /// <summary>
        /// Copy, for the current message, the attachment of <paramref name="name"/> to the <paramref name="target"/> <see cref="Stream"/>.
        /// </summary>
        Task CopyTo(string name, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Copy, for the current message, the attachment with the default name of <see cref="string.Empty"/> to the <paramref name="target"/> <see cref="Stream"/>.
        /// </summary>
        Task CopyTo(Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, for the current message, the attachment of <paramref name="name"/>.
        /// </summary>
        Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, all attachments for the current message.
        /// </summary>
        Task ProcessStreams(Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="byte"/> array, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
        /// </remarks>
        Task<byte[]> GetBytes(CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="byte"/> array, for the current message, the attachment of <paramref name="name"/>.
        /// </summary>
        Task<byte[]> GetBytes(string name, CancellationToken cancellation = default);

        /// <summary>
        /// Copy, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/> to the <paramref name="target"/> <see cref="Stream"/>.
        /// </summary>
        Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Copy, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/> to the <paramref name="target"/> <see cref="Stream"/>.
        /// </summary>
        Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
        /// </summary>
        Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Process with the delegate <paramref name="action"/>, all attachments for the for the message with <paramref name="messageId"/>.
        /// </summary>
        Task ProcessStreamsForMessage(string messageId, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="byte"/> array, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
        /// </remarks>
        Task<byte[]> GetBytesForMessage(string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="byte"/> array, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
        /// </remarks>
        Task<byte[]> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default);
    }
}