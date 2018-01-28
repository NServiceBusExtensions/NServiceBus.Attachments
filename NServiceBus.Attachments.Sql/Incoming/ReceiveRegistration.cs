using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveRegistration :
    RegisterStep
{
    public ReceiveRegistration(Func<Task<SqlConnection>> connectionFactory, StreamPersister persister)
        : base(
            stepId: $"{AssemblyHelper.Name}Receive",
            behavior: typeof(ReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: builder => new ReceiveBehavior(connectionFactory, persister))
    {
    }
}