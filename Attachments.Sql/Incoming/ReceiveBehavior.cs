using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<SqlConnection>> connectionBuilder;
    IPersister persister;

    public ReceiveBehavior(Func<Task<SqlConnection>> connectionBuilder, IPersister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        using (var state = new SqlAttachmentState(connectionBuilder, persister))
        {
            context.Extensions.Set(state);
            await next().ConfigureAwait(false);
        }
    }
}