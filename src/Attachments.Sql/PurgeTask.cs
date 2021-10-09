using System.Data.Common;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Logging;

class PurgeTask :
    FeatureStartupTask
{
    static ILog log = LogManager.GetLogger("AttachmentPurgeTask");
    IPersister persister;
    Func<Task<DbConnection>> connectionFactory;

    public PurgeTask(IPersister persister, Func<Task<DbConnection>> connectionFactory)
    {
        this.persister = persister;
        this.connectionFactory = connectionFactory;
    }

    protected override async Task OnStart(IMessageSession session)
    {
        using var connection = await connectionFactory();
        var count = await persister.PurgeItems(connection, null, CancellationToken.None);
        log.DebugFormat($"Deleted {count} attachments");
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }
}