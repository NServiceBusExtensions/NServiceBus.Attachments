using NServiceBus.Attachments.FileShare;
using NServiceBus.Features;

class PurgeTask :
    FeatureStartupTask
{
    IPersister persister;

    public PurgeTask(IPersister persister) =>
        this.persister = persister;

    protected override Task OnStart(IMessageSession session, Cancel cancel = default)
    {
        persister.PurgeItems(cancel);
        return Task.CompletedTask;
    }

    protected override Task OnStop(IMessageSession session, Cancel cancel = default) =>
        Task.CompletedTask;
}