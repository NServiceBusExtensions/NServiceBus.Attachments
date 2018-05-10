using System.IO;
using NServiceBus;
using NServiceBus.Features;

public static class Helpers
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
        configuration.DisableFeature<TimeoutManager>();
        configuration.EnableInstallers();
        configuration.DisableFeature<MessageDrivenSubscriptions>();
        configuration.LimitMessageProcessingConcurrencyTo(10);
    }

    public static void DisableRetries(this EndpointConfiguration configuration)
    {
        var recoverability = configuration.Recoverability();
        recoverability.Immediate(x => x.NumberOfRetries(0));
        recoverability.Delayed(x => x.NumberOfRetries(0));
    }


    public static byte[] Buffer = new byte[1000 * 10 * 1000];
}