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

        internal IncomingAttachments(Lazy<SqlConnection> connectionFactory, string messageId)
        {
            this.connectionFactory = connectionFactory;
            this.messageId = messageId;
        }

        public Task CopyTo(string name, Stream target)
        {
            var connection = connectionFactory.Value;
            return StreamPersister.CopyTo(name, target, connection, messageId);
        }
    }
}