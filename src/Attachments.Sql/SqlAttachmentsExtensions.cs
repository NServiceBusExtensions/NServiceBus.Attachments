using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Settings;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to enable and configure attachments.
    /// </summary>
    public static class SqlAttachmentsExtensions
    {
        /// <summary>
        /// Enable SQL attachments for this endpoint.
        /// </summary>
        public static AttachmentSettings EnableAttachments(
            this EndpointConfiguration configuration,
            Func<Task<DbConnection>> connectionFactory,
            GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNull(connectionFactory, nameof(connectionFactory));
            var settings = configuration.GetSettings();
            var attachments = new AttachmentSettings(connectionFactory, timeToKeep);
            return SetAttachments(configuration, settings, attachments);
        }

        /// <summary>
        /// Enable SQL attachments for this endpoint.
        /// </summary>
        public static AttachmentSettings EnableAttachments(
            this EndpointConfiguration configuration,
            string connection,
            GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Guard.AgainstNullOrEmpty(connection, nameof(connection));
            var settings = configuration.GetSettings();
            var attachments = new AttachmentSettings(() => OpenConnection(connection), timeToKeep);
            return SetAttachments(configuration, settings, attachments);
        }

        static AttachmentSettings SetAttachments(EndpointConfiguration configuration, SettingsHolder settings, AttachmentSettings attachments)
        {
            settings.Set(attachments);
            configuration.EnableFeature<AttachmentFeature>();
            configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
            return attachments;
        }

        static async Task<DbConnection> OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }
}
