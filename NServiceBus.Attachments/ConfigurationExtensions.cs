using System;
using System.Data.SqlClient;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class AttachmentsConfigurationExtensions
    {
        public static void EnableAttachments(this EndpointConfiguration configuration, Func<SqlConnection> connectionBuilder, string schema = "dbo",string tableName = "Attachments")
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            var pipeline = configuration.Pipeline;
            var streamPersister = new StreamPersister(schema,tableName);
            pipeline.Register(new StreamReceiveRegistration(connectionBuilder, streamPersister));
            pipeline.Register(new StreamSendRegistration(connectionBuilder, streamPersister));
        }
    }
}