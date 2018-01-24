using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachments
    {
        Lazy<SqlConnection> connectionFactory;
        string messageId;
        StreamPersister streamPersister;

        internal IncomingAttachments(Lazy<SqlConnection> connectionFactory, string messageId, StreamPersister streamPersister)
        {
            this.connectionFactory = connectionFactory;
            this.messageId = messageId;
            this.streamPersister = streamPersister;
        }

        public Task CopyTo(string name, Stream target)
        {
            var connection = connectionFactory.Value;
            return streamPersister.CopyTo(messageId, name, connection, target);
        }

        public Task ProcessStream(string name, Func<Stream, Task> action)
        {
            var connection = connectionFactory.Value;
            return streamPersister.ProcessStream(messageId, name, connection, action);
        }

        public async Task<byte[]> GetBytes(string name)
        {
            using (var memoryStream = new MemoryStream())
            {
                await CopyTo(name, memoryStream).ConfigureAwait(false);
                memoryStream.Position = 0;
                return memoryStream.GetBuffer();
            }
        }
    }
}