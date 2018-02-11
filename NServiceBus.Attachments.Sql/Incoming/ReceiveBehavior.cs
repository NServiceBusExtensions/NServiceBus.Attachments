using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<CancellationToken, Task<SqlConnection>> connectionBuilder;
    Persister persister;

    public ReceiveBehavior(Func<CancellationToken, Task<SqlConnection>> connectionBuilder, Persister persister)
    {
        this.connectionBuilder = connectionBuilder;
        this.persister = persister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        SqlConnection sqlConnection = null;
        var connectionFactory = new Lazy<Func<CancellationToken, Task<SqlConnection>>>(
             () =>
            {
                return async x =>
                {
                    return sqlConnection = await connectionBuilder(x).ConfigureAwait(false);
                };
            });

        context.Extensions.Set(
            new AttachmentReceiveState
            {
                Persister = persister,
                ConnectionFactory = x => connectionFactory.Value(x)
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