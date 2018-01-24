using System;

namespace NServiceBus.Attachments
{
    public delegate TimeSpan GetTimeToKeep(TimeSpan? messageTimeToBeReceived);
}