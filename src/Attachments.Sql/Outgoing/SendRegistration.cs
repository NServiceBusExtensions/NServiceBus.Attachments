using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class SendRegistration :
    RegisterStep
{
    public SendRegistration(Func<Cancel, Task<SqlConnection>> connectionFactory, IPersister persister, GetTimeToKeep timeToKeep)
        : base(
            stepId: $"{AssemblyHelper.Name}Send",
            behavior: typeof(SendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: _ => new SendBehavior(connectionFactory, persister, timeToKeep))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}