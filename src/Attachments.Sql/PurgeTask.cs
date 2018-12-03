using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;

class PurgeTask: FeatureStartupTask
{
    IPersister persister;
    Func<Task<SqlConnection>> connectionFactory;

    public PurgeTask(IPersister persister, Func<Task<SqlConnection>> connectionFactory)
    {
        this.persister = persister;
        this.connectionFactory = connectionFactory;
    }
    protected override async Task OnStart(IMessageSession session)
    {
        using (var connection = await connectionFactory())
        {
            await persister.PurgeItems(connection, null, CancellationToken.None).ConfigureAwait(false);
        }
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }
}