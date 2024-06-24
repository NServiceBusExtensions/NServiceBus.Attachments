using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    //TODO: remove?
    /// <inheritdoc />
    public virtual async Task ReadAllMessageInfo(SqlConnection connection, SqlTransaction? transaction, string messageId, Func<AttachmentInfo, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = GetReadInfoCommand(connection, transaction, messageId);
        using var reader = await command.ExecuteReaderAsync(cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            var info = new AttachmentInfo(
                messageId: messageId,
                name: reader.GetString(1),
                created: reader.GetDateTime(2),
                expiry: reader.GetDateTime(3),
                metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(4)));
            await action(info, cancel);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<(Guid, string)> ReadAllMessageNames(
        SqlConnection connection,
        SqlTransaction? transaction,
        string messageId,
        [EnumeratorCancellation] Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = GetReadInfoCommand(connection, transaction, messageId);
        using var reader = await command.ExecuteReaderAsync(cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            yield return (reader.GetGuid(0), reader.GetString(1));
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(
        SqlConnection connection,
        SqlTransaction? transaction,
        string messageId,
        [EnumeratorCancellation] Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = GetReadInfoCommand(connection, transaction, messageId);
        using var reader = await command.ExecuteReaderAsync(cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            yield return new(
                messageId: messageId,
                name: reader.GetString(1),
                created: reader.GetDateTime(2),
                expiry: reader.GetDateTime(3),
                metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(4)));
        }
    }

    /// <inheritdoc />
    public virtual async Task ReadAllInfo(SqlConnection connection, SqlTransaction? transaction, Func<AttachmentInfo, Cancel, Task> action, Cancel cancel = default)
    {
        using var command = GetReadInfosCommand(connection, transaction);
        using var reader = await command.ExecuteReaderAsync(cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            var info = new AttachmentInfo(
                messageId: reader.GetString(1),
                name: reader.GetString(2),
                created: reader.GetDateTime(3),
                expiry: reader.GetDateTime(4),
                metadata: MetadataSerializer.Deserialize(reader.GetStringOrNull(5)));
            await action(info, cancel);
        }
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AttachmentInfo>> ReadAllInfo(SqlConnection connection, SqlTransaction? transaction, Cancel cancel = default)
    {
        var list = new ConcurrentBag<AttachmentInfo>();
        await ReadAllInfo(
            connection,
            transaction,
            (info, _) =>
            {
                list.Add(info);
                return Task.CompletedTask;
            },
            cancel);
        return list;
    }

    SqlCommand GetReadInfosCommand(SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            select
                Id,
                MessageId,
                Name,
                Created,
                Expiry,
                Metadata
            from {table}
            order by Created
            """;
        return command;
    }

    SqlCommand GetReadNamesCommand(SqlConnection connection, SqlTransaction? transaction, string messageId)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            select
                Id,
                Name
            from {table}
            where
                MessageIdLower = lower(@MessageId)
            order by Created
            """;
        command.AddParameter("MessageId", messageId);

        return command;
    }

    SqlCommand GetReadInfoCommand(SqlConnection connection, SqlTransaction? transaction, string messageId)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            select
                Id,
                Name,
                Created,
                Expiry,
                Metadata
            from {table}
            where
                MessageIdLower = lower(@MessageId)
            """;
        command.AddParameter("MessageId", messageId);

        return command;
    }
}