using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(stream, nameof(stream));
            return Save(connection, transaction, messageId, name, expiry, stream,metadata, cancellation);
        }

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveBytes(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            return Save(connection, transaction, messageId, name, expiry, bytes, metadata, cancellation);
        }

        async Task Save(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, object stream, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellation = default)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $@"
insert into {table}
(
    MessageId,
    Name,
    Expiry,
    Data,
    Metadata
)
values
(
    @MessageId,
    @Name,
    @Expiry,
    @Data,
    @Metadata
)";
                command.AddParameter("MessageId", messageId);
                command.AddParameter("Name", name);
                command.AddParameter("Expiry", expiry);
                command.AddBinary("Data", stream);
                command.AddParameter("Metadata", MetadataSerializer.Serialize(metadata));

                // Send the data to the server asynchronously
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }
    }
}