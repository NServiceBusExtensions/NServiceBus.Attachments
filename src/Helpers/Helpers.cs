﻿public static class Helpers
{
    public static void PurgeDirectory(string directory)
    {
        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            Directory.Delete(subDirectory, true);
        }

        foreach (var file in Directory.EnumerateFiles(directory))
        {
            File.Delete(file);
        }
    }

    public static void ApplySharedPerfConfig(this EndpointConfiguration configuration)
    {
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.SendFailedMessagesTo("error");
        configuration.PurgeOnStartup(true);
        configuration.EnableInstallers();
        configuration.LimitMessageProcessingConcurrencyTo(10);
    }

    public static void DisableRetries(this EndpointConfiguration configuration)
    {
        var recoverability = configuration.Recoverability();
        recoverability.Immediate(_ => _.NumberOfRetries(0));
        recoverability.Delayed(_ => _.NumberOfRetries(0));
    }

    public static byte[] Buffer = new byte[1000 * 10 * 1000];
}