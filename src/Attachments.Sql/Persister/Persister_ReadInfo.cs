using System.Data.Common;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        //TODO: remove?
        /// <inheritdoc />
        public virtual async Task ReadAllMessageInfo(DbConnection connection, DbTransaction? transaction, string messageId, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            using var command = GetReadInfoCommand(connection, transaction, messageId);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                AttachmentInfo info = new(
                    messageId: messageId,
                    name: reader.GetString(1),
                    expiry: reader.GetDateTime(2),
                    metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(3)));
                var task = action(info);
                Guard.ThrowIfNullReturned(null, null, task);
                await task;
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<(Guid, string)> ReadAllMessageNames(
            DbConnection connection,
            DbTransaction? transaction,
            string messageId,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            using var command = GetReadInfoCommand(connection, transaction, messageId);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                yield return (reader.GetGuid(0), reader.GetString(1));
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(
            DbConnection connection,
            DbTransaction? transaction,
            string messageId,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            using var command = GetReadInfoCommand(connection, transaction, messageId);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                yield return new AttachmentInfo(
                    messageId: messageId,
                    name: reader.GetString(1),
                    expiry: reader.GetDateTime(2),
                    metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(3)));
            }
        }

        /// <inheritdoc />
        public virtual async Task ReadAllInfo(DbConnection connection, DbTransaction? transaction, Func<AttachmentInfo, Task> action, CancellationToken cancellation = default)
        {
            using var command = GetReadInfosCommand(connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                AttachmentInfo info = new(
                    messageId: reader.GetString(1),
                    name: reader.GetString(2),
                    expiry: reader.GetDateTime(3),
                    metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(4)));
                var task = action(info);
                Guard.ThrowIfNullReturned(null, null, task);
                await task;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<AttachmentInfo>> ReadAllInfo(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default)
        {
            ConcurrentBag<AttachmentInfo> list = new();
            await ReadAllInfo(
                connection,
                transaction,
                info =>
                {
                    list.Add(info);
                    return Task.CompletedTask;
                },
                cancellation);
            return list;
        }

        DbCommand GetReadInfosCommand(DbConnection connection, DbTransaction? transaction)
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
from {table}";
            return command;
        }

        DbCommand GetReadNamesCommand(DbConnection connection, DbTransaction? transaction, string messageId)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    Id,
    Name
from {table}
where
    MessageIdLower = lower(@MessageId)";
            command.AddParameter("MessageId", messageId);

            return command;
        }
        DbCommand GetReadInfoCommand(DbConnection connection, DbTransaction? transaction, string messageId)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    Id,
    Name,
    Expiry,
    Metadata
from {table}
where
    MessageIdLower = lower(@MessageId)";
            command.AddParameter("MessageId", messageId);

            return command;
        }
    }
}