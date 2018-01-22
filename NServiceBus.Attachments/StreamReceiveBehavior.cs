using System;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class StreamReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<IInvokeHandlerContext, ConnectionAndTransaction> connectionBuilder;

    public StreamReceiveBehavior(Func<IInvokeHandlerContext, ConnectionAndTransaction> connectionBuilder)
    {
        this.connectionBuilder = connectionBuilder;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var connectionFactory = new Lazy<ConnectionAndTransaction>(() => connectionBuilder(context));
        try
        {
            var incomingAttachments = new IncomingAttachments(
                connectionFactory: connectionFactory,
                messageId: context.MessageId);
            context.Extensions.Set(incomingAttachments);
            await next()
                .ConfigureAwait(false);
        }
        finally
        {
            if (connectionFactory.IsValueCreated)
            {
                connectionFactory.Value.Dispose();
            }
        }
    }
}