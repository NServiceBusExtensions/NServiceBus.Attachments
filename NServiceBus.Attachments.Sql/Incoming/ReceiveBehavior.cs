using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<Task<SqlConnection>> connectionBuilder;
    StreamPersister streamPersister;

    public ReceiveBehavior(Func<Task<SqlConnection>> connectionBuilder, StreamPersister streamPersister)
    {
        this.connectionBuilder = connectionBuilder;
        this.streamPersister = streamPersister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        SqlConnection sqlConnection = null;
        var connectionFactory = new Lazy<Task<SqlConnection>>(
            async () =>
            {
                return sqlConnection = await connectionBuilder().ConfigureAwait(false);
            });

        Func<Task<SqlConnection>> factory = () => connectionFactory.Value;
        context.SetConnectionFactory(factory);
        try
        {
            await Inner(context, next, factory);
        }
        finally
        {
            sqlConnection?.Dispose();
        }
    }

    Task Inner(IInvokeHandlerContext context, Func<Task> next, Func<Task<SqlConnection>> factory)
    {
        var incomingAttachments = new MessageAttachments(
            sqlConnectionFactory: factory,
            messageId: context.MessageId,
            streamPersister: streamPersister);
        context.Extensions.Set(incomingAttachments);
        return next();
    }
}