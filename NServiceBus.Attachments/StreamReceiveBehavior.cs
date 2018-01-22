using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class StreamReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<SqlConnection> connectionBuilder;

    public StreamReceiveBehavior(Func<SqlConnection> connectionBuilder)
    {
        this.connectionBuilder = connectionBuilder;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var lazy = new Lazy<Task<SqlConnection>>(async () =>
        {
            var sqlConnection = connectionBuilder();
            await sqlConnection.OpenAsync();
            return sqlConnection;
        });

        var incomingAttachments = new IncomingAttachments(lazy, context.MessageId);
        context.Extensions.Set(incomingAttachments);
        await next()
            .ConfigureAwait(false);
        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }
}