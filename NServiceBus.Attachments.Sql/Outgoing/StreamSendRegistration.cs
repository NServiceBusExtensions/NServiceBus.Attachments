using System;
using System.Data.SqlClient;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class StreamSendRegistration :
    RegisterStep
{
    public StreamSendRegistration(Func<SqlConnection> connectionBuilder, StreamPersister streamPersister, GetTimeToKeep timeToKeep)
        : base(
            stepId: $"{AssemblyHelper.Name}StreamSend",
            behavior: typeof(StreamSendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: builder => new StreamSendBehavior(connectionBuilder, streamPersister, timeToKeep))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}