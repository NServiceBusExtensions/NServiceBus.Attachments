using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task<int> DeleteAllAttachments(SqlConnection connection, SqlTransaction? transaction, Cancel cancel = default)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            delete from {table}
            select @@ROWCOUNT
            """;
        return (int) (await command.ExecuteScalarAsync(cancel))!;
    }

    public virtual async Task<int> DeleteAttachments(string messageId, SqlConnection connection, SqlTransaction? transaction, Cancel cancel = default)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            delete from {table} where MessageIdLower = lower(@MessageId)
            select @@ROWCOUNT
            """;
        command.AddParameter("MessageId", messageId);
        return (int) (await command.ExecuteScalarAsync(cancel))!;
    }
}