using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class StreamReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Func<SqlConnection> connectionBuilder;
    StreamPersister streamPersister;

    public StreamReceiveBehavior(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister)
    {
        this.connectionBuilder = connectionBuilder;
        this.streamPersister = streamPersister;
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var connectionFactory = new Lazy<SqlConnection>(() =>
        {
            var sqlConnection = connectionBuilder();
            sqlConnection.Open();
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