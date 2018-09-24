using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<SqlConnection>> connectionBuilder;
    IPersister persister;
    bool useTransportSqlConnectivity;

    public ReceiveBehavior(Func<Task<SqlConnection>> connectionBuilder, IPersister persister, bool useTransportSqlConnectivity)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
        this.useTransportSqlConnectivity = useTransportSqlConnectivity;
    }

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var state = BuildState(context);
        context.Extensions.Set(state);
        return next();
    }

    SqlAttachmentState BuildState(IInvokeHandlerContext context)
    {
        if (useTransportSqlConnectivity)
        {
            if (context.Extensions.TryGet<TransportTransaction>(out var transaction))
            {
                if (transaction.TryGet<SqlTransaction>(out var sqlTransaction))
                {
                    return new SqlAttachmentState(sqlTransaction, persister);
                }

                if (transaction.TryGet<SqlConnection>(out var sqlConnection))
                {
                    return new SqlAttachmentState(sqlConnection, persister);
                }
            }
            throw new Exception($"{nameof(AttachmentSettings.UseTransportConnectivity)} was configured but no {nameof(TransportTransaction)} could be found");
        }

        return new SqlAttachmentState(connectionBuilder, persister);
    }
}