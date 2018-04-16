using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    public partial class Persister
    {
        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStreams(string messageId, SqlConnection connection, SqlTransaction transaction, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            using (var command = CreateGetDatasCommand(messageId, connection, transaction))
            using (var reader = await command.ExecuteSequentialReader(cancellation).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
                {
                    cancellation.ThrowIfCancellationRequested();
                    var name = reader.GetString(0);
                    var length = reader.GetInt64(1);
                    using (var sqlStream = reader.GetStream(2))
                    {
                        var task = action(name, new AttachmentStream(sqlStream, length));
                        Guard.ThrowIfNullReturned(messageId,null, task);
                        await task.ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStream(string messageId, string name, SqlConnection connection, SqlTransaction transaction, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(action, nameof(action));
            using (var command = CreateGetDataCommand(messageId, name, connection, transaction))
            using (var reader = await command.ExecuteSequentialReader(cancellation).ConfigureAwait(false))
            {
                if (!await reader.ReadAsync(cancellation).ConfigureAwait(false))
                {
                    throw ThrowNotFound(messageId, name);
                }

                var length = reader.GetInt64(0);
                using (var sqlStream = reader.GetStream(1))
                {
                    var task = action(new AttachmentStream(sqlStream, length));
                    Guard.ThrowIfNullReturned(messageId, name, task);
                    await task.ConfigureAwait(false);
                }
            }
        }
    }
}