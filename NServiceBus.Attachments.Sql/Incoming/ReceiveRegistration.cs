using System;
using System.Data.SqlClient;
using NServiceBus.Pipeline;

class ReceiveRegistration :
    RegisterStep
{
    public ReceiveRegistration(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister)
        : base(
            stepId: $"{AssemblyHelper.Name}Receive",
            behavior: typeof(ReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: builder => new ReceiveBehavior(connectionBuilder, streamPersister))
    {
    }
}