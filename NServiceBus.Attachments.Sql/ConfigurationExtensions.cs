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
        public static void EnableAttachments(
            this EndpointConfiguration configuration,
            Func<Task<SqlConnection>> connectionBuilder,
            GetTimeToKeep timeToKeep,
            bool runCleanTask = true,
            string schema = "dbo",
            string tableName = "Attachments",
            bool disableInstaller = false)
        {
            Guard.AgainstSqlDelimiters(nameof(schema), schema);
            Guard.AgainstSqlDelimiters(nameof(tableName), tableName);
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            var settings = configuration.GetSettings();
            settings.Set<AttachmentSettings>(new AttachmentSettings(connectionBuilder, runCleanTask, schema, tableName, disableInstaller, timeToKeep));
            configuration.EnableFeature<AttachmentFeature>();
        }
    }
}