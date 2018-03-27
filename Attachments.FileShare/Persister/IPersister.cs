using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    public interface IPersister
    {
        /// <summary>
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentMetadata"/> for all attachments.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        IEnumerable<AttachmentMetadata> ReadAllMetadata(CancellationToken cancellation = default);

        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        void DeleteAllAttachments();

        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        void CleanupItemsOlderThan(DateTime dateTime, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Reads a byte array for an attachment.
        /// </summary>
        Task<byte[]> GetBytes(string messageId, string name, CancellationToken cancellation = default);

        /// <summary>
        /// Returns an open stream pointing to an attachment.
        /// </summary>
        Stream GetStream(string messageId, string name);

        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        Task ProcessStreams(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        Task ProcessStream(string messageId, string name, Func<Stream, Task> action, CancellationToken cancellation = default);
    }
}