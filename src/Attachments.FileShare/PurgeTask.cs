using NServiceBus;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Features;

class PurgeTask :
    FeatureStartupTask
{
    IPersister persister;

    public PurgeTask(IPersister persister)
    {
        this.persister = persister;
    }
    protected override Task OnStart(IMessageSession session)
    {
        persister.PurgeItems();
        return Task.CompletedTask;
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }
}