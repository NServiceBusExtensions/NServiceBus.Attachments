using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task ProcessStreams(string messageId, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = CreateGetDatasCommand(messageId, connection, transaction);
        using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var length = reader.GetInt64(1);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            using var sqlStream = reader.GetStream(3);
            using var attachment = new AttachmentStream(name, sqlStream, length, metadata);
            await action(attachment, cancellation);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessStream(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancellation);
        if (!await reader.ReadAsync(cancellation))
        {
            throw ThrowNotFound(messageId, name);
        }

        var length = reader.GetInt64(0);
        var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
        using var sqlStream = reader.GetStream(2);
        using var attachment = new AttachmentStream(name, sqlStream, length, metadata);
        await action(attachment, cancellation);
    }

    /// <inheritdoc />
    public virtual async Task ProcessByteArrays(string messageId, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = CreateGetDatasCommand(messageId, connection, transaction);
        using var reader = await command.ExecuteReaderAsync(cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            var bytes = (byte[]) reader.GetValue(3);
            var attachment = new AttachmentBytes(name, bytes, metadata);
            await action(attachment, cancellation);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessByteArray(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancellation);
        if (!await reader.ReadAsync(cancellation))
        {
            throw ThrowNotFound(messageId, name);
        }

        var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(1));
        var bytes = (byte[]) reader.GetValue(2);
        var attachment = new AttachmentBytes(name, bytes, metadata);
        await action(attachment, cancellation);
    }
}