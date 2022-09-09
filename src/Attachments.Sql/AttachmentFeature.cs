using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.ObjectBuilder;

class AttachmentFeature :
    Feature
{
    static ILog log = LogManager.GetLogger("AttachmentFeature");

    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<AttachmentSettings>();

        var connectionFactory = settings.ConnectionFactory;
        var pipeline = context.Pipeline;
        var persister = new Persister(settings.Table);
        pipeline.Register(new ReceiveRegistration(connectionFactory, persister, settings.UseTransport, settings.UseSynchronizedStorage));
        if (settings.UseTransport)
        {
            pipeline.Register(new DeleteBehaviorRegistration(connectionFactory, persister));
        }

        pipeline.Register(new SendRegistration(connectionFactory, persister, settings.TimeToKeep));
        if (context.Settings.PurgeOnStartup())
        {
            context.RegisterStartupTask(_ => new PurgeTask(persister, settings.ConnectionFactory));
        }

        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(builder => CreateCleaner(settings, persister, builder));
        }
    }

    static Cleaner CreateCleaner(AttachmentSettings settings, IPersister persister, IBuilder builder) =>
        new(
            async token =>
            {
                await using var connection = await settings.ConnectionFactory();
                var count = await persister.CleanupItemsOlderThan(connection, null, DateTime.UtcNow, token);
                log.Debug($"Deleted {count} attachments during cleanup");
            },
            criticalError: builder.Build<CriticalError>().Raise,
            frequencyToRunCleanup: TimeSpan.FromHours(1),
            timer: new AsyncTimer());
}