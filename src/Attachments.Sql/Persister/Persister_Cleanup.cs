using System.Data.Common;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual async Task<int> CleanupItemsOlderThan(DbConnection connection, DbTransaction? transaction, DateTime dateTime, CancellationToken cancellation = default)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
delete from {table} where expiry < @date
select @@ROWCOUNT";
            command.AddParameter("date", dateTime);

            var result = await command.ExecuteScalarAsync(cancellation);
            return (int)result!;
        }

        /// <inheritdoc />
        public virtual async Task<int> PurgeItems(DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
if exists (
    select * from sys.objects
    where
        object_id = object_id('{table}')
        and type in ('U')
)
begin

delete from {table}

end
select @@ROWCOUNT";
            return (int)(await command.ExecuteScalarAsync(cancellation))!;
        }
    }
}