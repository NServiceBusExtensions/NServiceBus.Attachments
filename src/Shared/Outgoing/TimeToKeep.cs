namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
;

/// <summary>
/// Helpers for defining how long to keep an attachment.
/// </summary>
/// <seealso cref="TimeToKeep"/>
public static class TimeToKeep
{
    /// <summary>
    /// The recommended default for how long to keep an attachment.
    /// If <paramref name="messageTimeToBeReceived"/> is defined then use double that value; otherwise use 10 days.
    /// </summary>
    public static TimeSpan Default(TimeSpan? messageTimeToBeReceived)
    {
        if (messageTimeToBeReceived is null)
        {
            return TimeSpan.FromDays(10);
        }

        return TimeSpan.FromTicks(messageTimeToBeReceived.Value.Ticks * 2);
    }
}