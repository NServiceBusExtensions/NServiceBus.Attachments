using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class SendRegistration :
    RegisterStep
{
    public SendRegistration(Func<Task<SqlConnection>> connectionFactory, IPersister persister, GetTimeToKeep timeToKeep, bool useTransportSqlConnectivity)
        : base(
            stepId: $"{AssemblyHelper.Name}Send",
            behavior: typeof(SendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: builder => new SendBehavior(connectionFactory, persister, timeToKeep, useTransportSqlConnectivity))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}