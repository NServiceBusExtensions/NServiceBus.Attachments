using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStreams(string messageId, DbConnection connection, DbTransaction? transaction, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            await using var command = CreateGetDatasCommand(messageId, connection, transaction);
            await using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var name = reader.GetString(0);
                var length = reader.GetInt64(1);
                var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
                await using var sqlStream = reader.GetStream(3);
                await using var attachmentStream = new AttachmentStream(name, sqlStream, length, metadata);
                var task = action(name, attachmentStream);
                Guard.ThrowIfNullReturned(messageId, null, task);
                await task;
            }
        }

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStream(string messageId, string name, DbConnection connection, DbTransaction? transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
            await using var reader = await command.ExecuteSequentialReader(cancellation);
            if (!await reader.ReadAsync(cancellation))
            {
                throw ThrowNotFound(messageId, name);
            }

            var length = reader.GetInt64(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
            await using var sqlStream = reader.GetStream(2);
            await using var attachmentStream = new AttachmentStream(name, sqlStream, length, metadata);
            var task = action(attachmentStream);
            Guard.ThrowIfNullReturned(messageId, name, task);
            await task;
        }
    }
}