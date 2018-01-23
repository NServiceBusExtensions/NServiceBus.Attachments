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
    }
}