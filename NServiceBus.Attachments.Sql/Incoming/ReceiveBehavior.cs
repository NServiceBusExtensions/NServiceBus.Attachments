using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<SqlConnection> connectionBuilder;
    StreamPersister streamPersister;

    public ReceiveBehavior(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister)
    {
        this.connectionBuilder = connectionBuilder;
        this.streamPersister = streamPersister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var connectionFactory = new Lazy<Task<SqlConnection>>(async () =>
        {
            var sqlConnection = connectionBuilder();
            await sqlConnection.OpenAsync()
                .ConfigureAwait(false);
            return sqlConnection;
        });
        try
        {
            var incomingAttachments = new IncomingAttachments(
                connectionFactory: connectionFactory,
                messageId: context.MessageId,
                streamPersister: streamPersister);
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