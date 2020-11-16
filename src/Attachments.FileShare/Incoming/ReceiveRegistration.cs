using NServiceBus.Attachments.FileShare;
using NServiceBus.Pipeline;

class ReceiveRegistration :
    RegisterStep
{
    public ReceiveRegistration(IPersister persister)
        : base(
            stepId: $"{AssemblyHelper.Name}Receive",
            behavior: typeof(ReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: _ => new ReceiveBehavior(persister))
    {
    }
}