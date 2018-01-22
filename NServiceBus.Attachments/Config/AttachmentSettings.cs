using System;
using System.Data.SqlClient;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

namespace NServiceBus.Attachments
{
    public class AttachmentSettings
    {
        Func<SqlConnection> connectionBuilder;
        bool useSqlTransportContext;

        public void UseSqlConnection(Func<SqlConnection> connectionBuilder)
        {
            Guard.AgainstNull(connectionBuilder, nameof(connectionBuilder));
            if (useSqlTransportContext)
            {
                throw new Exception($"{nameof(UseSqlTransportContext)} is already defined.");
            }

            this.connectionBuilder = connectionBuilder;
        }

        public void UseSqlTransportContext()
        {
            if (connectionBuilder != null)
            {
                throw new Exception($"{nameof(UseSqlConnection)} is already defined.");
            }

            useSqlTransportContext = true;
        }

        internal Func<IInvokeHandlerContext, ConnectionAndTransaction> GetConnectionAndTransaction()
        {
            if (useSqlTransportContext)
            {
                return ReadFromHandlerContext;
            }

            return ConstructFromBuilder;
        }

        ConnectionAndTransaction ConstructFromBuilder(IInvokeHandlerContext arg)
        {
            var sqlConnection = connectionBuilder();
            sqlConnection.Open();
            return new ConnectionAndTransaction(sqlConnection, null, false);
        }

        ConnectionAndTransaction ReadFromHandlerContext(IInvokeHandlerContext context)
        {
            var transportTransaction = context.Extensions.Get<TransportTransaction>();
            transportTransaction.TryGet<SqlConnection>(out var transportSqlConnection);
            transportTransaction.TryGet<SqlTransaction>(out var transportSqlTransaction);
            if (transportSqlTransaction == null)
            {
                throw new Exception("Could not extract SqlTransport connection.");
            }

            return new ConnectionAndTransaction(transportSqlConnection, transportSqlTransaction, false);
        }
    }
}