using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<SqlConnection>> connectionBuilder;
    IPersister persister;
    bool useTransport;
    bool useSynchronizedStorage;

    public ReceiveBehavior(Func<Task<SqlConnection>> connectionBuilder, IPersister persister, bool useTransport, bool useSynchronizedStorage)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
        this.useTransport = useTransport;
        this.useSynchronizedStorage = useSynchronizedStorage;
    }

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var state = BuildState(context);
        context.Extensions.Set(state);
        return next();
    }

    SqlAttachmentState BuildState(IInvokeHandlerContext context)
    {
        if (useSynchronizedStorage)
        {
            var session = context.SynchronizedStorageSession;
            if (session != null)
            {
                var propertyInfo = session.GetType().GetProperty("Connection",BindingFlags.NonPublic|BindingFlags.Instance);
                var value = propertyInfo.GetValue(session);
                Debug.WriteLine(session);
            }
        }
        if (useTransport)
        {
            if (context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
            {
                if (transportTransaction.TryGet<Transaction>(out var transaction))
                {
                    return new SqlAttachmentState(transaction, connectionBuilder, persister);
                }

                if (transportTransaction.TryGet<SqlTransaction>(out var sqlTransaction))
                {
                    return new SqlAttachmentState(sqlTransaction, persister);
                }

                if (transportTransaction.TryGet<SqlConnection>(out var sqlConnection))
                {
                    return new SqlAttachmentState(sqlConnection, persister);
                }
            }
            throw new Exception($"{nameof(AttachmentSettings.UseTransportConnectivity)} was configured but no {nameof(TransportTransaction)} could be found");
        }

        return new SqlAttachmentState(connectionBuilder, persister);
    }
}