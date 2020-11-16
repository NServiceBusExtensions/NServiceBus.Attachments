using NServiceBus.Attachments.FileShare;
using NServiceBus.Pipeline;

class SendRegistration :
    RegisterStep
{
    public SendRegistration(IPersister persister, GetTimeToKeep timeToKeep)
        : base(
            stepId: $"{AssemblyHelper.Name}Send",
            behavior: typeof(SendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: _ => new SendBehavior(persister, timeToKeep))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}