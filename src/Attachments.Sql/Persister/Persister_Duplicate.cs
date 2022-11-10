using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<(Guid, string)>> Duplicate(string sourceMessageId, SqlConnection connection, SqlTransaction? transaction, string targetMessageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        using var command = CreateGetDuplicateCommand(sourceMessageId, targetMessageId, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
        var names = new List<(Guid, string)>();
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            names.Add((reader.GetGuid(0), reader.GetString(1)));
        }

        return names;
    }

    /// <inheritdoc />
    public virtual async Task<Guid> Duplicate(string sourceMessageId, string name, SqlConnection connection, SqlTransaction? transaction, string targetMessageId, string targetName, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        Guard.AgainstNullOrEmpty(targetName, nameof(targetName));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDuplicateCommandWithRename(sourceMessageId, name, targetMessageId, targetName, connection, transaction);
        return (Guid) (await command.ExecuteScalarAsync(cancellation))!;
    }

    /// <inheritdoc />
    public virtual async Task<Guid> Duplicate(string sourceMessageId, string name, SqlConnection connection, SqlTransaction? transaction, string targetMessageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDuplicateCommand(sourceMessageId, name, targetMessageId, connection, transaction);
        return (Guid) (await command.ExecuteScalarAsync(cancellation))!;
    }

    SqlCommand CreateGetDuplicateCommand(string sourceMessageId, string targetMessageId, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
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
output inserted.ID, inserted.Name
select
    @TargetMessageId,
    Name,
    Expiry,
    Data,
    Metadata
from {table}
where
    MessageIdLower = lower(@SourceMessageId);
";
        command.AddParameter("SourceMessageId", sourceMessageId);
        command.AddParameter("TargetMessageId", targetMessageId);
        return command;
    }

    SqlCommand CreateGetDuplicateCommandWithRename(string sourceMessageId, string name, string targetMessageId, string targetName, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
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
select
    @TargetMessageId,
    @TargetName,
    Expiry,
    Data,
    Metadata
from {table}
where
    NameLower = lower(@Name) and
    MessageIdLower = lower(@SourceMessageId);
";
        command.AddParameter("Name", name);
        command.AddParameter("SourceMessageId", sourceMessageId);
        command.AddParameter("TargetMessageId", targetMessageId);
        command.AddParameter("TargetName", targetName);
        return command;
    }

    SqlCommand CreateGetDuplicateCommand(string sourceMessageId, string name, string targetMessageId, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
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
select
    @TargetMessageId,
    Name,
    Expiry,
    Data,
    Metadata
from {table}
where
    NameLower = lower(@Name) and
    MessageIdLower = lower(@SourceMessageId);
";
        command.AddParameter("Name", name);
        command.AddParameter("SourceMessageId", sourceMessageId);
        command.AddParameter("TargetMessageId", targetMessageId);
        return command;
    }
}