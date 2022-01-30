namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
;

/// <summary>
/// Defines a contract for getting a <see cref="TimeSpan"/> to keep an attachment.
/// </summary>
public delegate TimeSpan GetTimeToKeep(TimeSpan? messageTimeToBeReceived);

static class TimeToKeepEx
{
    public static Func<TimeSpan?, TimeSpan>? ToFunc(this GetTimeToKeep? timeToKeep)
    {
        if (timeToKeep is null)
        {
            return null;
        }
        return x => timeToKeep(x);
    }
}