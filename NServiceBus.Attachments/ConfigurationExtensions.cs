using System;
using System.Data.SqlClient;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control what messages are audited.
    /// </summary>
    public static class AttachmentsConfigurationExtensions
    {
        public static void EnableAttachments(this EndpointConfiguration configuration, Func<SqlConnection> connectionBuilder, bool runCleanTask = true, string schema = "dbo", string tableName = "Attachments")
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            configuration.GetSettings().Set<AttachmentSettings>(new AttachmentSettings(connectionBuilder, runCleanTask, schema, tableName));
            configuration.EnableFeature<AttachmentsFeature>();
            var pipeline = configuration.Pipeline;
            var streamPersister = new StreamPersister(schema, tableName);
            pipeline.Register(new StreamReceiveRegistration(connectionBuilder, streamPersister));
            pipeline.Register(new StreamSendRegistration(connectionBuilder, streamPersister));
        }
    }
}