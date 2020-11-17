using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class PhysicalBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    Func<Task<DbConnection>> connectionBuilder;
    IPersister persister;

    public PhysicalBehavior(Func<Task<DbConnection>> connectionBuilder, IPersister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }
    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        await next();

        if (context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
        {
            if (transportTransaction.TryGet<Transaction>(out var transaction))
            {
                using var connection = await connectionBuilder();
                connection.EnlistTransaction(transaction);
                await persister.DeleteAttachments(context.MessageId, connection, null);
            }
            if (transportTransaction.TryGet("System.Data.SqlClient.SqlTransaction", out DbTransaction dbTransaction))
            {
                await persister.DeleteAttachments(context.MessageId,dbTransaction.Connection, dbTransaction);
            }
        }
    }
}