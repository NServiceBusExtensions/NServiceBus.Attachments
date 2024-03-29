﻿public class Usage
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

    static void DisableCleanupTask(EndpointConfiguration configuration)
    {
        #region DisableCleanupTask

        var attachments = configuration.EnableAttachments(
            fileShare: "networkSharePath",
            timeToKeep: TimeToKeep.Default);
        attachments.DisableCleanupTask();

        #endregion
    }
}