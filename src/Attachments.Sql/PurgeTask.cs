using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Logging;

class PurgeTask :
    FeatureStartupTask
{
    static ILog log = LogManager.GetLogger("AttachmentPurgeTask");
    IPersister persister;
    Func<Cancel, Task<SqlConnection>> connectionFactory;

    public PurgeTask(IPersister persister, Func<Cancel, Task<SqlConnection>> connectionFactory)
    {
        this.persister = persister;
        this.connectionFactory = connectionFactory;
    }

    protected override async Task OnStart(IMessageSession session, Cancel cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        var count = await persister.PurgeItems(connection, null, Cancel.None);
        log.DebugFormat($"Deleted {count} attachments");
    }

    protected override Task OnStop(IMessageSession session, Cancel cancel = default) =>
        Task.CompletedTask;
}