using NServiceBus.Settings;

static class Extensions
{
    public static bool PurgeOnStartup(this ReadOnlySettings settings)
    {
        if (settings.TryGet("Transport.PurgeOnStartup", out bool purgeOnStartup))
        {
            return purgeOnStartup;
        }
        return false;
    }
}