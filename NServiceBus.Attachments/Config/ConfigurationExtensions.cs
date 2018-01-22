using System;
using NServiceBus.Attachments;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class AttachmentsConfigurationExtensions
    {
        public static AttachmentSettings EnableAttachments(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(endpointConfiguration, nameof(endpointConfiguration));
            var settings = endpointConfiguration.GetSettings();
            var attachmentSettings = new AttachmentSettings();
            settings.Set<AttachmentSettings>(attachmentSettings);
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new StreamReceiveRegistration(attachmentSettings));
            pipeline.Register(new StreamSendRegistration(attachmentSettings));
            return attachmentSettings;
        }

        internal static AttachmentSettings GetSettings(this ContextBag settings)
        {
            if (settings.TryGet<AttachmentSettings>(out var value))
            {
                return value;
            }

            throw new Exception("AttachmentSettings must be defined.");
        }
    }
}