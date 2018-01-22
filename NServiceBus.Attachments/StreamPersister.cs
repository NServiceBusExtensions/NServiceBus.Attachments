using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

static class StreamPersister
{
    public static async Task SaveStream(SqlConnection connection, string messageId, string name, TimeSpan streamTimeToKeep, Stream stream)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
insert into Attachments
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
            parameters.Add("@Expiry", SqlDbType.DateTime).Value = DateTime.UtcNow.Add(streamTimeToKeep);
            parameters.Add("@Data", SqlDbType.Binary, -1).Value = stream;

            // Send the data to the server asynchronously
            await command.ExecuteNonQueryAsync();
        }
    }
    public static async Task CopyTo(string name, Stream target, SqlConnection connectionConnection, string messageId)
    {
        using (var command = connectionConnection.CreateCommand())
        {
            command.CommandText = @"
select
    Data
from Attachments
where
    Name=@Name and
    MessageId=@MessageId";
            var parameters = command.Parameters;
            parameters.AddWithValue("Name", name);
            parameters.AddWithValue("MessageId", messageId);

            // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
            // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
            using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                if (await reader.ReadAsync())
                {
                    if (!await reader.IsDBNullAsync(0))
                    {
                        using (var data = reader.GetStream(0))
                        {
                            // Asynchronously copy the stream from the server to the file we just created
                            await data.CopyToAsync(target);
                            return;
                        }
                    }
                }
            }
        }

        throw new Exception("Could not find");
    }
}