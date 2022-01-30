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
        Guard.AgainstNullOrEmpty(fileShare, nameof(fileShare));
        var settings = configuration.GetSettings();
        AttachmentSettings attachments = new(fileShare, timeToKeep);
        settings.Set(attachments);
        configuration.EnableFeature<AttachmentFeature>();
        configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
        return attachments;
    }
}