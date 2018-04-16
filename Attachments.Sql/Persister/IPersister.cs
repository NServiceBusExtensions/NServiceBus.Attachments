using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
    public interface IPersister
    {
        /// <summary>
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveBytes(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        Task ReadAllMessageInfo(SqlConnection connection, SqlTransaction transaction, string messageId, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        Task<IReadOnlyCollection<AttachmentInfo>> ReadAllMessageInfo(SqlConnection connection, SqlTransaction transaction, string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        Task ReadAllInfo(SqlConnection connection, SqlTransaction transaction, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        Task<IReadOnlyCollection<AttachmentInfo>> ReadAllInfo(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        Task DeleteAllAttachments(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        Task CleanupItemsOlderThan(SqlConnection connection, SqlTransaction transaction, DateTime dateTime, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        Task CopyTo(string messageId, string name, SqlConnection connection, SqlTransaction transaction, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Reads a byte array for an attachment.
        /// </summary>
        Task<byte[]> GetBytes(string messageId, string name, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Returns an open stream pointing to an attachment.
        /// </summary>
        Task<AttachmentStream> GetStream(string messageId, string name, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation);

        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        Task ProcessStreams(string messageId, SqlConnection connection, SqlTransaction transaction, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        Task ProcessStream(string messageId, string name, SqlConnection connection, SqlTransaction transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);
    }
}