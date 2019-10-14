using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
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
        Task SaveStream(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveBytes(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="value"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task SaveString(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, string value, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        Task ReadAllMessageInfo(DbConnection connection, DbTransaction? transaction, string messageId, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        Task<IReadOnlyCollection<AttachmentInfo>> ReadAllMessageInfo(DbConnection connection, DbTransaction? transaction, string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        Task ReadAllInfo(DbConnection connection, DbTransaction? transaction, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        Task<IReadOnlyCollection<AttachmentInfo>> ReadAllInfo(DbConnection connection, DbTransaction transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        Task DeleteAllAttachments(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        Task CleanupItemsOlderThan(DbConnection connection, DbTransaction? transaction, DateTime dateTime, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes all items.
        /// </summary>
        Task PurgeItems(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        Task CopyTo(string messageId, string name, DbConnection connection, DbTransaction? transaction, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to a different message.
        /// </summary>
        Task Duplicate(string sourceMessageId, string name, DbConnection connection, DbTransaction? transaction, string targetMessageId, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to a different message.
        /// </summary>
        Task Duplicate(string sourceMessageId, string name, DbConnection connection, DbTransaction? transaction, string targetMessageId, string targetName, CancellationToken cancellation = default);

        /// <summary>
        /// Copies all attachments to a different message.
        /// </summary>
        Task Duplicate(string sourceMessageId, DbConnection connection, DbTransaction? transaction, string targetMessageId, CancellationToken cancellation = default);

        /// <summary>
        /// Reads all <see cref="AttachmentBytes"/>s for an attachment.
        /// </summary>
        IAsyncEnumerable<AttachmentBytes> GetBytes(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads all <see cref="AttachmentString"/>s for an attachment.
        /// </summary>
        IAsyncEnumerable<AttachmentString> GetStrings(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads all <see cref="AttachmentStream"/>s to an attachment.
        /// </summary>
        IAsyncEnumerable<AttachmentStream> GetStreams(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads an <see cref="AttachmentBytes"/> for an attachment.
        /// </summary>
        Task<AttachmentBytes> GetBytes(string messageId, string name, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads an <see cref="AttachmentString"/> for an attachment.
        /// </summary>
        Task<AttachmentString> GetString(string messageId, string name, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads an <see cref="AttachmentStream"/> for an attachment.
        /// </summary>
        Task<AttachmentStream> GetStream(string messageId, string name, DbConnection connection, DbTransaction? transaction,  bool disposeConnectionOnStreamDispose, CancellationToken cancellation);

        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        Task ProcessStreams(string messageId, DbConnection connection, DbTransaction? transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        Task ProcessStream(string messageId, string name, DbConnection connection, DbTransaction? transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);
    }
}