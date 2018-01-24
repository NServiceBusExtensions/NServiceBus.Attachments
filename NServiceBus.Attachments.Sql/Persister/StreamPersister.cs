using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

class StreamPersister
{
    string fullTableName;

    public StreamPersister(string schema, string tableName)
    {
        fullTableName = $"[{schema}].[{tableName}]";
    }

    public async Task SaveStream(SqlConnection connection, SqlTransaction transaction, string messageId, string name, DateTime expiry, Stream stream)
    {
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = $@"
insert into {fullTableName}
(
    MessageId,
    Name,
    Expiry,
    Data
)
values
(
    @MessageId,
    @Name,
    @Expiry,
    @Data
)";
            var parameters = command.Parameters;
            parameters.Add("@MessageId", SqlDbType.NVarChar).Value = messageId;
            parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
            parameters.Add("@Expiry", SqlDbType.DateTime2).Value = expiry;
            parameters.Add("@Data", SqlDbType.Binary, -1).Value = stream;

            // Send the data to the server asynchronously
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

    public IEnumerable<ReadRow> ReadAllMetadata(SqlConnection connection)
    {
        using (var command = GetReadMetadataCommand(connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                yield return new ReadRow(
                    id: reader.GetGuid(0),
                    messageId: reader.GetString(1),
                    name: reader.GetString(2),
                    expiry: reader.GetDateTime(3));
            }
        }
    }

    SqlCommand GetReadMetadataCommand(SqlConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = $@"
select
    [Id],
    [MessageId],
    [Name],
    [Expiry]
from {fullTableName}";
        return command;
    }

    public void DeleteAllRows(SqlConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"delete from {fullTableName}";
            command.ExecuteNonQuery();
        }
    }

    public void CleanupItemsOlderThan(SqlConnection connection, DateTime dateTime)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"delete from {fullTableName} where expiry < @date";
            command.Parameters.AddWithValue("date", dateTime);
            command.ExecuteNonQuery();
        }
    }

    public async Task CopyTo(string messageId, string name, SqlConnection connection, Stream target)
    {
        using (var command = CreateGetDataCommand(messageId, name, connection))
        using (var reader = await ExecuteSequentialReader(command).ConfigureAwait(false))
        {
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                using (var data = reader.GetStream(0))
                {
                    await data.CopyToAsync(target).ConfigureAwait(false);
                    return;
                }
            }
        }

        ThrowNotFound(messageId, name);
    }

    public async Task ProcessStreams(string messageId, SqlConnection connection, Func<string, Stream, Task> action)
    {
        using (var command = CreateGetDatasCommand(messageId, connection))
        using (var reader = await ExecuteSequentialReader(command).ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var name = reader.GetString(0);
                using (var data = reader.GetStream(1))
                {
                    await action(name, data).ConfigureAwait(false);
                }
            }
        }
    }

    public async Task ProcessStream(string messageId, string name, SqlConnection connection, Func<Stream, Task> action)
    {
        using (var command = CreateGetDataCommand(messageId, name, connection))
        using (var reader = await ExecuteSequentialReader(command).ConfigureAwait(false))
        {
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                using (var data = reader.GetStream(0))
                {
                    await action(data).ConfigureAwait(false);
                    return;
                }
            }
        }

        ThrowNotFound(messageId, name);
    }

    static void ThrowNotFound(string messageId, string name)
    {
        throw new Exception($"Could not find attachment. MessageId:{messageId}, Name:{name}");
    }

    // The reader needs to be executed with SequentialAccess to enable network streaming
    // Otherwise ReadAsync will buffer the entire BLOB in memory which can cause scalability issues or OutOfMemoryExceptions
    static Task<SqlDataReader> ExecuteSequentialReader(SqlCommand command)
    {
        return command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
    }

    SqlCommand CreateGetDataCommand(string messageId, string name, SqlConnection connection)
    {
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = $@"
select
    Data
from {fullTableName}
where
    Name=@Name and
    MessageId=@MessageId";
        var parameters = sqlCommand.Parameters;
        parameters.AddWithValue("Name", name);
        parameters.AddWithValue("MessageId", messageId);
        return sqlCommand;
    }

    SqlCommand CreateGetDatasCommand(string messageId, SqlConnection connection)
    {
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = $@"
select
    Name,
    Data
from {fullTableName}
where
    MessageId=@MessageId";
        var parameters = sqlCommand.Parameters;
        parameters.AddWithValue("MessageId", messageId);
        return sqlCommand;
    }
}