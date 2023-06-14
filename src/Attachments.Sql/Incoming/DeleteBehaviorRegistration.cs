using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class DeleteBehaviorRegistration :
    RegisterStep
{
    public DeleteBehaviorRegistration(Func<Cancellation, Task<SqlConnection>> connectionBuilder, IPersister persister)
        : base(
            stepId: $"{AssemblyHelper.Name}DeleteBehavior",
            behavior: typeof(DeleteBehavior),
            description: "Performs cleanup of attachments for the current message.",
            factoryMethod: _ => new DeleteBehavior(connectionBuilder, persister))
    {
    }
}