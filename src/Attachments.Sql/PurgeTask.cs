using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Logging;

class PurgeTask(IPersister persister, Func<Cancel, Task<SqlConnection>> connectionFactory) :
    FeatureStartupTask
{
    static ILog log = LogManager.GetLogger("AttachmentPurgeTask");

    protected override async Task OnStart(IMessageSession session, Cancel cancel = default)
    {
        await using var connection = await connectionFactory(cancel);
        var count = await persister.PurgeItems(connection, null, Cancel.None);
        log.DebugFormat($"Deleted {count} attachments");
    }

    protected override Task OnStop(IMessageSession session, Cancel cancel = default) =>
        Task.CompletedTask;
}