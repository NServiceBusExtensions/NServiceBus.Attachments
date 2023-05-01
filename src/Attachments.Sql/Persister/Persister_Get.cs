using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task<AttachmentString> GetString(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Encoding? encoding = null, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
        if (await reader.ReadAsync(cancellation))
        {
            var metadataString = reader.GetStringOrNull(1);
            var metadata = MetadataSerializer.Deserialize(metadataString);
            encoding = MetadataSerializer.GetEncoding(encoding, metadata);
            //TODO: read string directly
            var bytes = (byte[]) reader[2];
            using var memoryStream = new MemoryStream(bytes);
            using var streamReader = new StreamReader(memoryStream, encoding, true);
            return new(name, streamReader.ReadToEnd(), metadata);
        }

        throw ThrowNotFound(messageId, name);
    }

    /// <inheritdoc />
    public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
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
    public virtual async Task<MemoryStream> GetMemoryStream(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
        if (await reader.ReadAsync(cancellation))
        {
            var bytes = (byte[]) reader[2];

            return new(bytes);
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
        Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
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
        [EnumeratorCancellation] Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = CreateGetDatasCommand(messageId, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
        while (await reader.ReadAsync(cancellation))
        {
            cancellation.ThrowIfCancellationRequested();
            var name = reader.GetString(0);
            var length = reader.GetInt64(1);
            var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
            using var sqlStream = reader.GetStream(3);
            yield return new(name, sqlStream, length, metadata);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentBytes> GetBytes(
        string messageId,
        SqlConnection connection,
        SqlTransaction? transaction,
        [EnumeratorCancellation] Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        using var command = CreateGetDatasCommand(messageId, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
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
    public virtual async IAsyncEnumerable<AttachmentString> GetStrings(string messageId, SqlConnection connection, SqlTransaction? transaction, Encoding? encoding = null, [EnumeratorCancellation] Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        encoding = encoding.Default();
        using var command = CreateGetDatasCommand(messageId, connection, transaction);
        using var reader = await command.ExecuteSequentialReader(cancellation);
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
            return new(name, sqlStream, length, metadata, command.DisposeAsync, reader.DisposeAsync, command.Connection!.DisposeAsync);
        }

        return new(name, sqlStream, length, metadata, command.DisposeAsync, reader.DisposeAsync);
    }

    SqlCommand CreateGetDataCommand(string messageId, string name, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"""
            
            select
                datalength(Data),
                Metadata,
                Data
            from {table}
            where
                NameLower = lower(@Name) and
                MessageIdLower = lower(@MessageId)
            """;
        command.AddParameter("Name", name);
        command.AddParameter("MessageId", messageId);
        return command;
    }

    SqlCommand CreateGetDatasCommand(string messageId, SqlConnection connection, SqlTransaction? transaction)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"""
            select
                Name,
                datalength(Data),
                Metadata,
                Data
            from {table}
            where
                MessageIdLower = lower(@MessageId)
            """;
        command.AddParameter("MessageId", messageId);
        return command;
    }
}