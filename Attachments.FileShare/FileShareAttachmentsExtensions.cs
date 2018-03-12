using NServiceBus.Attachments.FileShare;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class FileShareAttachmentsExtensions
    {
        /// <summary>
        /// Enable SQL attachments for this endpoint.
        /// </summary>
        public static FileShareAttachmentSettings EnableAttachments(
            this EndpointConfiguration configuration,
            string fileShare,
            GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNullOrEmpty(fileShare, nameof(fileShare));
            var settings = configuration.GetSettings();
            var attachments = new FileShareAttachmentSettings(fileShare, timeToKeep);
            settings.Set<FileShareAttachmentSettings>(attachments);
            configuration.EnableFeature<AttachmentFeature>();
            configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
            return attachments;
        }
    }
}