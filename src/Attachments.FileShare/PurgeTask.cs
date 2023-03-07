using NServiceBus.Attachments.FileShare;
using NServiceBus.Features;

class PurgeTask :
    FeatureStartupTask
{
    IPersister persister;

    public PurgeTask(IPersister persister) =>
        this.persister = persister;

    protected override Task OnStart(IMessageSession session, Cancellation cancellation = default)
    {
        persister.PurgeItems(cancellation);
        return Task.CompletedTask;
    }

    protected override Task OnStop(IMessageSession session, Cancellation cancellation = default) =>
        Task.CompletedTask;
}