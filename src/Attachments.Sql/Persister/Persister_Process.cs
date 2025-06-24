using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task ProcessStreams(string messageId, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        await using var command = CreateGetDatasCommand(messageId, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var length = reader.GetInt64(1);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            await using var sqlStream = reader.GetStream(3);
            await using var attachment = new AttachmentStream(name, sqlStream, length, metadata);
            await action(attachment, cancel);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessStream(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancel);
        if (!await reader.ReadAsync(cancel))
        {
            throw ThrowNotFound(messageId, name);
        }

        var length = reader.GetInt64(0);
        var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
        await using var sqlStream = reader.GetStream(2);
        await using var attachment = new AttachmentStream(name, sqlStream, length, metadata);
        await action(attachment, cancel);
    }

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(string messageId, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        await using var command = CreateGetDatasCommand(messageId, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(cancel);
        while (await reader.ReadAsync(cancel))
        {
            cancel.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            var bytes = (byte[]) reader.GetValue(3);
            var attachment = new AttachmentBytes(name, bytes, metadata);
            await action(attachment, cancel);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessByteArray(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancel);
        if (!await reader.ReadAsync(cancel))
        {
            throw ThrowNotFound(messageId, name);
        }

        var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
        var bytes = (byte[]) reader.GetValue(2);
        var attachment = new AttachmentBytes(name, bytes, metadata);
        await action(attachment, cancel);
    }
}