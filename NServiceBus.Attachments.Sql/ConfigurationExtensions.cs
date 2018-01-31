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
            Func<Task<SqlConnection>> connectionFactory,
            GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNull(connectionFactory, nameof(connectionFactory));
            var settings = configuration.GetSettings();
            var attachments = new AttachmentSettings(connectionFactory,timeToKeep);
            settings.Set<AttachmentSettings>(attachments);
            configuration.EnableFeature<AttachmentFeature>();
            configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
            return attachments;
        }
    }
}