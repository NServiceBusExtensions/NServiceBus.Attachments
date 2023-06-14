using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Logging;

class PurgeTask :
    FeatureStartupTask
{
    static ILog log = LogManager.GetLogger("AttachmentPurgeTask");
    IPersister persister;
    Func<Cancellation, Task<SqlConnection>> connectionFactory;

    public PurgeTask(IPersister persister, Func<Cancellation, Task<SqlConnection>> connectionFactory)
    {
        this.persister = persister;
        this.connectionFactory = connectionFactory;
    }

    protected override async Task OnStart(IMessageSession session, Cancellation cancellation = default)
    {
        using var connection = await connectionFactory(cancellation);
        var count = await persister.PurgeItems(connection, null, Cancellation.None);
        log.DebugFormat($"Deleted {count} attachments");
    }

    protected override Task OnStop(IMessageSession session, Cancellation cancellation = default) =>
        Task.CompletedTask;
}