using System;
using System.Data.SqlClient;

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
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new StreamReceiveRegistration(connectionBuilder));
            pipeline.Register(new StreamSendRegistration(connectionBuilder));
        }
    }
}