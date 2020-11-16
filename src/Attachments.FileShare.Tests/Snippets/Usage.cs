using System;
using NServiceBus;
using NServiceBus.Attachments.FileShare;

public class Usage
{
    Usage(EndpointConfiguration configuration)
    {
        #region EnableAttachments

        configuration.EnableAttachments(
            fileShare: "networkSharePath",
            timeToKeep: _ => TimeSpan.FromDays(7));

        #endregion

        #region EnableAttachmentsRecommended

        configuration.EnableAttachments(
            fileShare: "networkSharePath",
            timeToKeep: TimeToKeep.Default);

        #endregion
    }

    void DisableCleanupTask(EndpointConfiguration configuration)
    {
        #region DisableCleanupTask

        var attachments = configuration.EnableAttachments(
            fileShare: "networkSharePath",
            timeToKeep: TimeToKeep.Default);
        attachments.DisableCleanupTask();

        #endregion
    }
}