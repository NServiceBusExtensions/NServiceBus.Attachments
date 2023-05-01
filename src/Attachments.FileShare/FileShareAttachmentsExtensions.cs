using NServiceBus.Attachments.FileShare;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus;

/// <summary>
/// Extensions to enable and configure attachments.
/// </summary>
public static class FileShareAttachmentsExtensions
{
    /// <summary>
    /// Enable SQL attachments for this endpoint.
    /// </summary>
    public static AttachmentSettings EnableAttachments(
        this EndpointConfiguration configuration,
        string fileShare,
        GetTimeToKeep timeToKeep)
    {
        Guard.AgainstNullOrEmpty(fileShare);
        var settings = configuration.GetSettings();
        var attachments = new AttachmentSettings(fileShare, timeToKeep);
        settings.Set(attachments);
        configuration.EnableFeature<AttachmentFeature>();
        configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
        return attachments;
    }
}