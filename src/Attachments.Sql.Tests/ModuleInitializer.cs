using NServiceBus.Logging;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        LogManager.UseFactory(NullLogger.Instance);
    }
}