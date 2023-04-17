using NServiceBus.Logging;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.InitializePlugins();
        LogManager.UseFactory(NullLogger.Instance);
    }
}