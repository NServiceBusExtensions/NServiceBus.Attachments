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

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        using (var state = BuildState(context))
        {
            context.Extensions.Set(state);
            context.SetPersister(persister);
            await next().ConfigureAwait(false);
        }
    }

    SqlAttachmentState BuildState(IInvokeHandlerContext context)
    {
        if (useTransportSqlConnectivity)
        {
            if (context.Extensions.TryGet<TransportTransaction>(out var transaction))
            {
                if (transaction.TryGet<SqlTransaction>(out var sqlTransaction))
                {
                    return new SqlAttachmentState(sqlTransaction);
                }

                if (transaction.TryGet<SqlConnection>(out var sqlConnection))
                {
                    return new SqlAttachmentState(sqlConnection);
                }
            }
        }

        return new SqlAttachmentState(connectionBuilder);
    }
}