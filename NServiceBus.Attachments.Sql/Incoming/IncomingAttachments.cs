using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    class IncomingAttachments: IIncomingAttachments
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
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(target, nameof(target));
            var connection = await connectionFactory.Value;
            await streamPersister.CopyTo(messageId, name, connection, target).ConfigureAwait(false);
        }

        public async Task ProcessStream(string name, Func<Stream, Task> action)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(action, nameof(action));
            var connection = await connectionFactory.Value;
            await streamPersister.ProcessStream(messageId, name, connection, action).ConfigureAwait(false);
        }

        public async Task ProcessStreams(Func<string, Stream, Task> action)
        {
            Guard.AgainstNull(action, nameof(action));
            var connection = await connectionFactory.Value;
            await streamPersister.ProcessStreams(messageId, connection, action).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytes(string name)
        {
            Guard.AgainstNull(name, nameof(name));
            var connection = await connectionFactory.Value;
            return await streamPersister.GetBytes(messageId, name, connection).ConfigureAwait(false);
        }
    }
}