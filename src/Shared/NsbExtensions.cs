using System;
using NServiceBus.Pipeline;
using NServiceBus.Settings;
using NServiceBus.Transport;

static class NsbExtensions
{
    public static bool PurgeOnStartup(this ReadOnlySettings settings)
    {
        if (settings.TryGet("Transport.PurgeOnStartup", out bool purgeOnStartup))
        {
            return purgeOnStartup;
        }

        return false;
    }

    public static IncomingMessage IncomingMessage(this IOutgoingLogicalMessageContext context)
    {
        if (context.TryGetIncomingPhysicalMessage(out var incomingMessage))
        {
            return incomingMessage;
        }
        throw new Exception("Expected IncomingPhysicalMessage to exist.");
    }

    public static string IncomingMessageId(this IOutgoingLogicalMessageContext context)
    {
        return context.IncomingMessage().MessageId;
    }
}