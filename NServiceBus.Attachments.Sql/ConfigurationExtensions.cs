using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class AttachmentsConfigurationExtensions
    {
        public static AttachmentSettings EnableAttachments(
            this EndpointConfiguration configuration,
            Func<Task<SqlConnection>> connectionBuilder,
            GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNull(connectionBuilder, nameof(connectionBuilder));
            var settings = configuration.GetSettings();
            var attachmentSettings = new AttachmentSettings(connectionBuilder,timeToKeep);
            settings.Set<AttachmentSettings>(attachmentSettings);
            configuration.EnableFeature<AttachmentFeature>();
            return attachmentSettings;
        }
    }
}