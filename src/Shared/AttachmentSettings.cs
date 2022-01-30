// ReSharper disable PartialTypeWithSinglePart
namespace NServiceBus.Attachments
#if FileShare
.FileShare
#elif Sql
.Sql
#endif
;

/// <summary>
/// All settings for attachments
/// </summary>
public partial class AttachmentSettings
{
    internal bool RunCleanTask = true;
    // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8618
#pragma warning disable 649
    internal readonly GetTimeToKeep TimeToKeep;
#pragma warning restore 649
#pragma warning restore CS8618

    /// <summary>
    /// Disable the attachment cleanup task.
    /// </summary>
    public void DisableCleanupTask()
    {
        RunCleanTask = false;
    }
}