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
        /// <inheritdoc />
        public virtual async Task<int> DeleteAllAttachments(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"delete from {table}
select @@ROWCOUNT";
            return (int)await command.ExecuteScalarAsync(cancellation);
        }

        public virtual async Task<int> DeleteAttachments(string messageId, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
delete from {table} where MessageIdLower = lower(@MessageId)
select @@ROWCOUNT";
            command.AddParameter("MessageId", messageId);
            return (int)await command.ExecuteScalarAsync(cancellation);
        }
    }
}