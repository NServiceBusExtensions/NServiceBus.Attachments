using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class DeleteBehaviorRegistration :
    RegisterStep
{
    public DeleteBehaviorRegistration(Func<Task<DbConnection>> connectionBuilder, IPersister persister)
        : base(
            stepId: $"{AssemblyHelper.Name}DeleteBehavior",
            behavior: typeof(DeleteBehavior),
            description: "Performs cleanup of attachments for the current message.",
            factoryMethod: _ => new DeleteBehavior(connectionBuilder, persister))
    {
    }
}