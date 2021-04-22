using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
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
        Task<Guid> SaveStream(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task<Guid> SaveBytes(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Saves <paramref name="value"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        Task<Guid> SaveString(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, string value, Encoding? encoding, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        Task ReadAllMessageInfo(DbConnection connection, DbTransaction? transaction, string messageId, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(DbConnection connection, DbTransaction? transaction, string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Reads the names for all attachments of a specific message.
        /// </summary>
        IAsyncEnumerable<(Guid,string)> ReadAllMessageNames(DbConnection connection, DbTransaction? transaction, string messageId, CancellationToken cancellation = default);

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
        Task<int> DeleteAllAttachments(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        Task<int> CleanupItemsOlderThan(DbConnection connection, DbTransaction? transaction, DateTime dateTime, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes all items.
        /// </summary>
        Task<int> PurgeItems(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        Task CopyTo(string messageId, string name, DbConnection connection, DbTransaction? transaction, Stream target, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to a different message.
        /// </summary>
        Task<Guid> Duplicate(string sourceMessageId, string name, DbConnection connection, DbTransaction? transaction, string targetMessageId, CancellationToken cancellation = default);

        /// <summary>
        /// Copies an attachment to a different message.
        /// </summary>
        Task<Guid> Duplicate(string sourceMessageId, string name, DbConnection connection, DbTransaction? transaction, string targetMessageId, string targetName, CancellationToken cancellation = default);

        /// <summary>
        /// Copies all attachments to a different message.
        /// </summary>
        Task<IReadOnlyCollection<(Guid, string)>> Duplicate(string sourceMessageId, DbConnection connection, DbTransaction? transaction, string targetMessageId, CancellationToken cancellation = default);

        /// <summary>
        /// Reads all <see cref="AttachmentBytes"/>s for an attachment.
        /// </summary>
        IAsyncEnumerable<AttachmentBytes> GetBytes(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes attachments for message.
        /// </summary>
        Task<int> DeleteAttachments(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default);

        /// <summary>
        /// Reads all <see cref="AttachmentString"/>s for an attachment.
        /// </summary>
        IAsyncEnumerable<AttachmentString> GetStrings(string messageId, DbConnection connection, DbTransaction? transaction, Encoding? encoding, CancellationToken cancellation = default);

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
        Task<AttachmentString> GetString(string messageId, string name, DbConnection connection, DbTransaction? transaction, Encoding? encoding, CancellationToken cancellation = default);

        /// <summary>
        /// Reads an <see cref="AttachmentStream"/> for an attachment.
        /// </summary>
        Task<AttachmentStream> GetStream(string messageId, string name, DbConnection connection, DbTransaction? transaction,  bool disposeConnectionOnStreamDispose, CancellationToken cancellation = default);

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