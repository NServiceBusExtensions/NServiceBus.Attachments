using System;
using System.Data.SqlClient;
using NServiceBus.Pipeline;

class StreamReceiveRegistration :
    RegisterStep
{
    public StreamReceiveRegistration(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister)
        : base(
            stepId: "StreamReceive",
            behavior: typeof(StreamReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: builder => new StreamReceiveBehavior(connectionBuilder, streamPersister))
    {
    }
}