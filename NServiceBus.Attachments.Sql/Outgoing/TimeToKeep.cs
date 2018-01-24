using System;

namespace NServiceBus.Attachments
{
    public static class TimeToKeep
    {
        public static TimeSpan Default(TimeSpan? messageTimeToBeReceived)
        {
            if (messageTimeToBeReceived == null)
            {
                return TimeSpan.FromDays(10);
            }

            return TimeSpan.FromTicks(messageTimeToBeReceived.Value.Ticks * 2);
        }
    }
}