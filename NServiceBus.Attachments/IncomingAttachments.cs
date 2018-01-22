using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachments
    {
        Lazy<ConnectionAndTransaction> connectionFactory;
        string messageId;

        internal IncomingAttachments(Lazy<ConnectionAndTransaction> connectionFactory, string messageId)
        {
            this.connectionFactory = connectionFactory;
            this.messageId = messageId;
        }

        public Task CopyTo(string name, Stream target)
        {
            var connection = connectionFactory.Value;
            var connectionConnection = connection.Connection;
            var transaction = connection.Transaction;
            return StreamPersister.CopyTo(name, target, connectionConnection, transaction, messageId);
        }
    }
}