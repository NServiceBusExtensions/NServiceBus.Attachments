using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachments
    {
        Lazy<Task<SqlConnection>> connectionFactory;
        string messageId;

        public IncomingAttachments(Lazy<Task<SqlConnection>> connectionFactory, string messageId)
        {
            this.connectionFactory = connectionFactory;
            this.messageId = messageId;
        }

        public async Task CopyTo(string name, Stream target)
        {
            var connection = await connectionFactory.Value;
            using (var command = connection.CreateCommand())
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
}