using System;
using NServiceBus;
using NServiceBus.Features;

class AttachmentsFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<AttachmentSettings>();
        var pipeline = context.Pipeline;
        var streamPersister = new StreamPersister(settings.Schema, settings.TableName);
        pipeline.Register(new StreamReceiveRegistration(settings.ConnectionBuilder, streamPersister));
        pipeline.Register(new StreamSendRegistration(settings.ConnectionBuilder, streamPersister));
        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(
                startupTaskFactory: b =>
                {
                    return new AttachmentCleaner(async token =>
                        {
                            using (var connection = settings.ConnectionBuilder())
                            {
                                await connection.OpenAsync(token);
                                streamPersister.CleanupItemsOlderThan(connection, DateTime.UtcNow);
                            }
                        },
                        criticalError: b.Build<CriticalError>().Raise,
                        frequencyToRunCleanup: TimeSpan.FromHours(1),
                        timer: new AsyncTimer());
                });
        }
    }
}