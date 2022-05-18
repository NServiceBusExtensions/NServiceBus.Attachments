using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task<AttachmentString> GetString(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Encoding? encoding = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstLongAttachmentName(name);
        encoding = encoding.Default();
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        if (await reader.ReadAsync(cancellation))
        {
            var metadataString = reader.GetStringOrNull(1);
            var metadata = MetadataSerializer.Deserialize(metadataString);
            //TODO: read string directly
            var bytes = (byte[]) reader[2];
            return new(name, encoding.GetString(bytes), metadata);
        }

        throw ThrowNotFound(messageId, name);
    }

    /// <inheritdoc />
    public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstLongAttachmentName(name);
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        if (await reader.ReadAsync(cancellation))
        {
            var metadataString = reader.GetStringOrNull(1);
            var metadata = MetadataSerializer.Deserialize(metadataString);
            var bytes = (byte[]) reader[2];

            return new(name, bytes, metadata);
        }

        throw ThrowNotFound(messageId, name);
    }

    /// <inheritdoc />
    public virtual async Task<AttachmentStream> GetStream(
        string messageId,
        string name,
        SqlConnection connection,
        SqlTransaction? transaction,
        bool disposeConnectionOnStreamDispose,
        CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstLongAttachmentName(name);
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        if (await reader.ReadAsync(cancellation))
        {
            return InnerGetStream(name, reader, command, disposeConnectionOnStreamDispose);
        }

        throw ThrowNotFound(messageId, name);
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentStream> GetStreams(
        string messageId,
        SqlConnection connection,
        SqlTransaction? transaction,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        await using var command = CreateGetDatasCommand(messageId, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var length = reader.GetInt64(1);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            await using var sqlStream = reader.GetStream(3);
            yield return new(name, sqlStream, length, metadata);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentBytes> GetBytes(
        string messageId,
        SqlConnection connection,
        SqlTransaction? transaction,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        await using var command = CreateGetDatasCommand(messageId, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            var bytes = (byte[]) reader[3];
            yield return new(name, bytes, metadata);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentString> GetStrings(string messageId, SqlConnection connection, SqlTransaction? transaction, Encoding? encoding = null, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        encoding = encoding.Default();
        await using var command = CreateGetDatasCommand(messageId, connection, transaction);
        await using var reader = await command.ExecuteSequentialReader(cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            //TODO: read string directly
            var bytes = (byte[]) reader[3];
            yield return new(name, encoding.GetString(bytes), metadata);
        }
    }

    static AttachmentStream InnerGetStream(string name, SqlDataReader reader, SqlCommand command, bool disposeConnection)
    {
        var length = reader.GetInt64(0);
        var metadataString = reader.GetStringOrNull(1);
        var sqlStream = reader.GetStream(2);
        var metadata = MetadataSerializer.Deserialize(metadataString);
        if (disposeConnection)
        {
            return new(name, sqlStream, length, metadata, command, reader, command.Connection!);
        }

        return new(name, sqlStream, length, metadata, command, reader);
    }

    SqlCommand CreateGetDataCommand(string messageId, string name, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $@"
select
    datalength(Data),
    Metadata,
    Data
from {table}
where
    NameLower = lower(@Name) and
    MessageIdLower = lower(@MessageId)";
        command.AddParameter("Name", name);
        command.AddParameter("MessageId", messageId);
        return command;
    }

    SqlCommand CreateGetDatasCommand(string messageId, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $@"
select
    Name,
    datalength(Data),
    Metadata,
    Data
from {table}
where
    MessageIdLower = lower(@MessageId)";
        command.AddParameter("MessageId", messageId);
        return command;
    }
}