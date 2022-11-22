using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Features;

class AttachmentFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var readOnlySettings = context.Settings;
        var settings = readOnlySettings.Get<AttachmentSettings>();

        var pipeline = context.Pipeline;
        var persister = new Persister(settings.FileShare);
        pipeline.Register(new ReceiveRegistration(persister));
        pipeline.Register(new SendRegistration(persister, settings.TimeToKeep));
        if (context.Settings.PurgeOnStartup())
        {
            context.RegisterStartupTask(_ => new PurgeTask(persister));
        }

        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(services => CreateCleaner(persister, services));
        }
    }

    static Cleaner CreateCleaner(IPersister persister, IServiceProvider services) =>
        new(token =>
            {
                persister.CleanupItemsOlderThan(DateTime.UtcNow, token);
                return Task.CompletedTask;
            },
            criticalError: services.GetRequiredService<CriticalError>().Raise,
            frequencyToRunCleanup: TimeSpan.FromHours(1),
            timer: new AsyncTimer());
}