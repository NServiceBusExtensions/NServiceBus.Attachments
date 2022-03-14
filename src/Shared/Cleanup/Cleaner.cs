﻿using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

class Cleaner :
    FeatureStartupTask
{
    public Cleaner(
        Func<CancellationToken, Task> cleanup,
        Action<string, Exception> criticalError,
        TimeSpan frequencyToRunCleanup,
        IAsyncTimer timer)
    {
        this.cleanup = cleanup;
        this.frequencyToRunCleanup = frequencyToRunCleanup;
        this.timer = timer;
        this.criticalError = criticalError;
    }

    protected override Task OnStart(IMessageSession? session)
    {
        var cleanupFailures = 0;
        timer.Start(
            callback: async (_, token) =>
            {
                await cleanup(token);
                cleanupFailures = 0;
            },
            interval: frequencyToRunCleanup,
            errorCallback: exception =>
            {
                log.Error("Error cleaning Attachment data", exception);
                cleanupFailures++;
                if (cleanupFailures >= 10)
                {
                    criticalError("Failed to clean expired Attachment records after 10 consecutive unsuccessful attempts. The most likely cause of this is connectivity issues with the database.", exception);
                    cleanupFailures = 0;
                }
            },
            delayStrategy: Task.Delay);
        return Task.CompletedTask;
    }

    protected override Task OnStop(IMessageSession? session) =>
        timer.Stop();

    IAsyncTimer timer;
    Action<string, Exception> criticalError;
    Func<CancellationToken, Task> cleanup;
    TimeSpan frequencyToRunCleanup;

    static ILog log = LogManager.GetLogger<Cleaner>();
}