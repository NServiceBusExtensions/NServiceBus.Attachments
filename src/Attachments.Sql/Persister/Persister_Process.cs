using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual async Task ProcessStreams(string messageId, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            await using var command = CreateGetDatasCommand(messageId, connection, transaction);
            await using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var name = reader.GetString(0);
                var length = reader.GetInt64(1);
                var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
                await using var sqlStream = reader.GetStream(3);
                await using AttachmentStream attachment = new(name, sqlStream, length, metadata);
                var task = action(attachment);
                Guard.ThrowIfNullReturned(messageId, null, task);
                await task;
            }
        }

        /// <inheritdoc />
        public virtual async Task ProcessStream(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
            await using var reader = await command.ExecuteSequentialReader(cancellation);
            if (!await reader.ReadAsync(cancellation))
            {
                throw ThrowNotFound(messageId, name);
            }

            var length = reader.GetInt64(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
            await using var sqlStream = reader.GetStream(2);
            await using AttachmentStream attachment = new(name, sqlStream, length, metadata);
            var task = action(attachment);
            Guard.ThrowIfNullReturned(messageId, name, task);
            await task;
        }
    }
}