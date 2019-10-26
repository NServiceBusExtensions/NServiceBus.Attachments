using NServiceBus.Logging;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        LogManager.UseFactory(NullLogger.Instance);
    }
}