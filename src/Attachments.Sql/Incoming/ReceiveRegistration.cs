using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class ReceiveRegistration :
    RegisterStep
{
    public ReceiveRegistration(Func<Task<DbConnection>> connectionFactory,
        IPersister persister,
        bool useTransport, bool useSynchronizedStorage)
        : base(
            stepId: $"{AssemblyHelper.Name}Receive",
            behavior: typeof(ReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: _ => new ReceiveBehavior(connectionFactory, persister, useTransport, useSynchronizedStorage))
    {
    }
}