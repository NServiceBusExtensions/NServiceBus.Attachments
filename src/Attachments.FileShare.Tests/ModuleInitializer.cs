using Newtonsoft.Json;
using NServiceBus.Logging;
using Verify;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        LogManager.UseFactory(NullLogger.Instance);
        SharedVerifySettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                serializerSettings.ContractResolver = new CustomContractResolver();
            });
        });
    }
}