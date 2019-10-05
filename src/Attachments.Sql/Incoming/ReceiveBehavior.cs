using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<DbConnection>> connectionBuilder;
    IPersister persister;
    bool useTransport;
    bool useSynchronizedStorage;
    StorageAccessor storageAccessor;

    public ReceiveBehavior(Func<Task<DbConnection>> connectionBuilder, IPersister persister, bool useTransport, bool useSynchronizedStorage)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
        this.useTransport = useTransport;
        this.useSynchronizedStorage = useSynchronizedStorage;
        storageAccessor = new StorageAccessor();
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
                if (storageAccessor.TryGetTransaction(session, out var transaction))
                {
                    return new SqlAttachmentState(transaction!, persister);
                }
                if (storageAccessor.TryGetConnection(session, out var connection))
                {
                    return new SqlAttachmentState(connection!, persister);
                }
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

                if (transportTransaction.TryGet("System.Data.SqlClient.SqlTransaction", out DbTransaction dbTransaction))
                {
                    return new SqlAttachmentState(dbTransaction, persister);
                }

                if (transportTransaction.TryGet("System.Data.SqlClient.SqlConnection", out DbConnection connection))
                {
                    return new SqlAttachmentState(connection, persister);
                }
            }
            else
            {
                throw new Exception($"{nameof(AttachmentSettings.UseTransportConnectivity)} was configured but no {nameof(TransportTransaction)} could be found");
            }
        }

        return new SqlAttachmentState(connectionBuilder, persister);
    }
}