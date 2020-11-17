using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class PhysicalBehaviorRegistration :
    RegisterStep
{
    public PhysicalBehaviorRegistration(Func<Task<DbConnection>> connectionBuilder, IPersister persister)
        : base(
            stepId: $"{AssemblyHelper.Name}PhysicalBehavior",
            behavior: typeof(PhysicalBehavior),
            description: "Performs cleanup",
            factoryMethod: _ => new PhysicalBehavior(connectionBuilder, persister))
    {
    }
}