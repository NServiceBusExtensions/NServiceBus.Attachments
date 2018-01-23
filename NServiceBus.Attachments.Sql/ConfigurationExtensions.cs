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
        public static void EnableAttachments(
            this EndpointConfiguration configuration,
            Func<SqlConnection> connectionBuilder,
            bool runCleanTask = true,
            string schema = "dbo",
            string tableName = "NServiceBusAttachments",
            bool runInstaller = true)
        {
            Guard.AgainstSqlDelimiters(nameof(schema), schema);
            Guard.AgainstSqlDelimiters(nameof(tableName), tableName);
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            var settings = configuration.GetSettings();
            settings.Set<AttachmentSettings>(new AttachmentSettings(connectionBuilder, runCleanTask, schema, tableName, runInstaller));
            configuration.EnableFeature<AttachmentsFeature>();
        }
    }
}