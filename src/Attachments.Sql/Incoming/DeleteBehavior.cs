using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Logging;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class DeleteBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    static ILog log = LogManager.GetLogger("AttachmentDeleteBehavior");
    Func<Task<DbConnection>> connectionBuilder;
    IPersister persister;

    public DeleteBehavior(Func<Task<DbConnection>> connectionBuilder, IPersister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        await next();

        if (!context.MessageHeaders.TryGetValue(Headers.MessageIntent, out var intent))
        {
            log.DebugFormat("Did not delete attachments for {0} since there is no message intent", context.MessageId);
            return;
        }

        if (intent != "Send" && intent != "Reply")
        {
            log.DebugFormat("Did not delete attachments for {0} since intent is {1}", context.MessageId, intent);
            return;
        }

        if (!context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
        {
            log.DebugFormat("Did not delete attachments for {0} since there is no TransportTransaction", context.MessageId);
            return;
        }

        if (transportTransaction.TryGet("System.Data.SqlClient.SqlTransaction", out DbTransaction dbTransaction))
        {
            log.DebugFormat("Deleting attachments for {0} using System.Data.SqlClient.SqlTransaction", context.MessageId);
            await persister.DeleteAttachments(context.MessageId, dbTransaction.Connection, dbTransaction);
            return;
        }

        if (transportTransaction.TryGet<Transaction>(out var transaction))
        {
            log.DebugFormat("Deleting attachments for {0} using Transactions.Transaction", context.MessageId);
            using var connection = await connectionBuilder();
            connection.EnlistTransaction(transaction);
            await persister.DeleteAttachments(context.MessageId, connection, null);
        }

        log.DebugFormat("Did not delete attachments for {0} since there is no Transactions.Transaction or System.Data.SqlClient.SqlTransaction", context.MessageId);
    }
}