using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    public partial class Persister
    {
        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        public virtual async Task ReadAllMessageInfo(SqlConnection connection, SqlTransaction transaction, string messageId, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            using (var command = GetReadInfoCommand(connection, transaction, messageId))
            using (var reader = await command.ExecuteSequentialReader(cancellation).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
                {
                    cancellation.ThrowIfCancellationRequested();
                    var info = new AttachmentInfo(
                        messageId: messageId,
                        name: reader.GetString(1),
                        expiry: reader.GetDateTime(2),
                        metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(3)));
                    var task = action(info);
                    Guard.ThrowIfNullReturned(null, null, task);
                    await task.ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        public virtual async Task<IReadOnlyCollection<AttachmentInfo>> ReadAllMessageInfo(SqlConnection connection, SqlTransaction transaction, string messageId, CancellationToken cancellation = default)
        {
            var list = new ConcurrentBag<AttachmentInfo>();
            await ReadAllMessageInfo(connection, transaction, messageId,
                    metadata =>
                    {
                        list.Add(metadata);
                        return Task.CompletedTask;
                    }, cancellation)
                .ConfigureAwait(false);
            return list;
        }

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        public virtual async Task ReadAllInfo(SqlConnection connection, SqlTransaction transaction, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            using (var command = GetReadInfosCommand(connection, transaction))
            using (var reader = await command.ExecuteSequentialReader(cancellation).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
                {
                    cancellation.ThrowIfCancellationRequested();
                    var info = new AttachmentInfo(
                        messageId: reader.GetString(1),
                        name: reader.GetString(2),
                        expiry: reader.GetDateTime(3),
                        metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(4)));
                    var task = action(info);
                    Guard.ThrowIfNullReturned(null, null, task);
                    await task.ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        public virtual async Task<IReadOnlyCollection<AttachmentInfo>> ReadAllInfo(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation = default)
        {
            var list = new ConcurrentBag<AttachmentInfo>();
            await ReadAllInfo(connection, transaction,
                    info =>
                    {
                        list.Add(info);
                        return Task.CompletedTask;
                    }, cancellation)
                .ConfigureAwait(false);
            return list;
        }

        SqlCommand GetReadInfosCommand(SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    Id,
    MessageId,
    Name,
    Expiry,
    Metadata
from {fullTableName}";
            return command;
        }

        SqlCommand GetReadInfoCommand(SqlConnection connection, SqlTransaction transaction, string messageId)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    Id,
    Name,
    Expiry,
    Metadata
from {fullTableName}
where
    MessageIdLower = lower(@MessageId)";
            command.AddParameter("MessageId", messageId);

            return command;
        }
    }
}