using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachments
    {
        Lazy<Task<SqlConnection>> connectionFactory;
        string messageId;
        StreamPersister streamPersister;

        internal IncomingAttachments(Lazy<Task<SqlConnection>> connectionFactory, string messageId, StreamPersister streamPersister)
        {
            this.connectionFactory = connectionFactory;
            this.messageId = messageId;
            this.streamPersister = streamPersister;
        }

        public async Task CopyTo(string name, Stream target)
        {
            var connection = await connectionFactory.Value;
            await streamPersister.CopyTo(messageId, name, connection, target).ConfigureAwait(false);
        }

        public async Task ProcessStream(string name, Func<Stream, Task> action)
        {
            var connection = await connectionFactory.Value;
            await streamPersister.ProcessStream(messageId, name, connection, action).ConfigureAwait(false);
        }

        public async Task ProcessStreams(Func<string, Stream, Task> action)
        {
            var connection = await connectionFactory.Value;
            await streamPersister.ProcessStreams(messageId, connection, action).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytes(string name)
        {
            var connection = await connectionFactory.Value;
            return await streamPersister.GetBytes(messageId, name, connection).ConfigureAwait(false);
        }
    }
}