﻿using System.Transactions;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Logging;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class DeleteBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    static ILog log = LogManager.GetLogger("AttachmentDeleteBehavior");
    Func<Task<SqlConnection>> connectionBuilder;
    IPersister persister;

    public DeleteBehavior(Func<Task<SqlConnection>> connectionBuilder, IPersister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        await next();

        var id = context.MessageId;
        var headers = context.MessageHeaders;

        if (!headers.ContainsKey("Attachments"))
        {
            //log.Debug($"Did not delete attachments for {id} since there is no 'Attachments' header");
            return;
        }

        if (!headers.TryGetValue(Headers.MessageIntent, out var intent))
        {
            //log.Debug($"Did not delete attachments for {id} since there is no message intent");
            return;
        }

        if (intent != "Send" && intent != "Reply")
        {
            //log.Debug($"Did not delete attachments for {id} since intent is {intent}");
            return;
        }

        if (!context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
        {
            //log.Debug($"Did not delete attachments for {id} since there is no TransportTransaction");
            return;
        }

        if (transportTransaction.TryGet("System.Data.SqlClient.SqlTransaction", out SqlTransaction dbTransaction))
        {
            var count = await persister.DeleteAttachments(id, dbTransaction.Connection!, dbTransaction);
            log.Debug($"Deleted {count} attachments for {id} using System.Data.SqlClient.SqlTransaction");
            return;
        }

        if (transportTransaction.TryGet<Transaction>(out var transaction))
        {
            using var connection = await connectionBuilder();
            connection.EnlistTransaction(transaction);
            var count = await persister.DeleteAttachments(id, connection, null);
            log.Debug($"Deleting {count} attachments for {id} using Transactions.Transaction");
        }

        //log.Debug($"Did not delete attachments for {id} since there is no Transactions.Transaction or System.Data.SqlClient.SqlTransaction");
    }
}