using Newtonsoft.Json;
using NServiceBus.Logging;
using VerifyTests;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        LogManager.UseFactory(NullLogger.Instance);
        VerifierSettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                serializerSettings.ContractResolver = new CustomContractResolver();
            });
        });
    }
}