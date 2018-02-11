using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;

class AttachmentFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<AttachmentSettings>();

        var pipeline = context.Pipeline;
        var persister = new Persister(settings.Schema, settings.TableName);
        pipeline.Register(new ReceiveRegistration(settings.ConnectionFactory, persister));

        pipeline.Register(new SendRegistration(settings.ConnectionFactory, persister, settings.TimeToKeep, settings.Cancellation));
        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(builder => CreateCleaner(settings, persister, builder));
        }
    }

    static Cleaner CreateCleaner(AttachmentSettings settings, Persister persister, IBuilder builder)
    {
        return new Cleaner(async token =>
            {
                using (var connection = await settings.ConnectionFactory(token).ConfigureAwait(false))
                {
                   await persister.CleanupItemsOlderThan(connection, null, DateTime.UtcNow, token).ConfigureAwait(false);
                }
            },
            criticalError: builder.Build<CriticalError>().Raise,
            frequencyToRunCleanup: TimeSpan.FromHours(1),
            timer: new AsyncTimer());
    }
}