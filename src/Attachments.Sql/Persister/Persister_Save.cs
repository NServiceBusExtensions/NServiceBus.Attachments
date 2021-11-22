using System.Data.Common;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual Task<Guid> SaveStream(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            stream.MoveToStart();
            return Save(connection, transaction, messageId, name, expiry, stream, metadata, cancellation);
        }

        /// <inheritdoc />
        public virtual Task<Guid> SaveBytes(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            return Save(connection, transaction, messageId, name, expiry, bytes, metadata, cancellation);
        }

        /// <inheritdoc />
        public virtual Task<Guid> SaveString(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, string value, Encoding? encoding = null, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            encoding = encoding.Default();
            return Save(connection, transaction, messageId, name, expiry, value.ToBytes(encoding), metadata, cancellation);
        }

        async Task<Guid> Save(DbConnection connection, DbTransaction? transaction, string messageId, string name, DateTime expiry, object stream, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            await using var command = connection.CreateCommand();
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
output inserted.ID
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
            command.AddParameter("Metadata", MetadataSerializer.Serialize(metadata));
            command.AddBinary("Data", stream);

            // Send the data to the server asynchronously
            return (Guid) (await command.ExecuteScalarAsync(cancellation))!;
        }
    }
}