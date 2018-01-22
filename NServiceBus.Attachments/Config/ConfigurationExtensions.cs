using System;
using System.Data.SqlClient;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class AttachmentsConfigurationExtensions
    {
        public static void EnableAttachments(this EndpointConfiguration endpointConfiguration, Func<SqlConnection> connectionBuilder)
        {
            Guard.AgainstNull(endpointConfiguration, nameof(endpointConfiguration));
            var settings = endpointConfiguration.GetSettings();
            settings.Set<AttachmentSettings>(new AttachmentSettings(connectionBuilder));
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new StreamReceiveRegistration(connectionBuilder));
            pipeline.Register(new StreamSendRegistration(connectionBuilder));
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