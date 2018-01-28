using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<SqlConnection>> connectionBuilder;
    StreamPersister persister;

    public ReceiveBehavior(Func<Task<SqlConnection>> connectionBuilder, StreamPersister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        SqlConnection sqlConnection = null;
        var connectionFactory = new Lazy<Task<SqlConnection>>(
            async () => { return sqlConnection = await connectionBuilder().ConfigureAwait(false); });

        context.Extensions.Set(
            new AttachmentReceiveState
            {
                Persister = persister,
                ConnectionFactory = () => connectionFactory.Value
            });
        try
        {
            await next();
        }
        finally
        {
            sqlConnection?.Dispose();
        }
    }
}