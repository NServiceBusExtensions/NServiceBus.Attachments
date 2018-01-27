using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
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
        var connectionFactory = new Lazy<Task<SqlConnection>>(() => connectionBuilder());

        try
        {
            await Inner(context, next, () => connectionFactory.Value);
        }
        finally
        {
            if (connectionFactory.IsValueCreated)
            {
                connectionFactory.Value.Dispose();
            }
        }
    }

    Task Inner(IInvokeHandlerContext context, Func<Task> next, Func<Task<SqlConnection>> factory)
    {
        var incomingAttachments = new IncomingAttachments(
            connectionFactory: factory,
            messageId: context.MessageId,
            streamPersister: streamPersister);
        context.Extensions.Set(incomingAttachments);
        return next();
    }
}