﻿// ReSharper disable PartialTypeWithSinglePart
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
    internal readonly GetTimeToKeep TimeToKeep = null!;

    /// <summary>
    /// Disable the attachment cleanup task.
    /// </summary>
    public void DisableCleanupTask() =>
        RunCleanTask = false;
}