using NServiceBus.Pipeline;

class UsedWhenNotEnabledRegistration :
    RegisterStep
{
    public UsedWhenNotEnabledRegistration()
        : base(
            stepId: $"{AssemblyHelper.Name}UsedWhenNotEnabled",
            behavior: typeof(UsedWhenNotEnabledBehavior),
            description: "Throws an exception if usage of attachments is detected but attachments is not configured")
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}