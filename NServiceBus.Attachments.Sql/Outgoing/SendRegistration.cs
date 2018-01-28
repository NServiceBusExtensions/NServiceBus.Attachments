using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class SendRegistration :
    RegisterStep
{
    public SendRegistration(Func<Task<SqlConnection>> connectionFactory, StreamPersister streamPersister, GetTimeToKeep timeToKeep)
        : base(
            stepId: $"{AssemblyHelper.Name}Send",
            behavior: typeof(SendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: builder => new SendBehavior(connectionFactory, streamPersister, timeToKeep))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}