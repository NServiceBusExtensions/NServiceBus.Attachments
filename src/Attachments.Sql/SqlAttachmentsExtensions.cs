using System.Data;
using System.Data.Common;
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
            Func<DbConnection> connectionFactory,
            GetTimeToKeep timeToKeep)
        {
            var dbConnection = connectionFactory();
            if (dbConnection.State == ConnectionState.Open)
            {
                throw new("This overload of EnableAttachments expects `Func<DbConnection> connectionFactory` to return a un-opened DbConnection.");
            }
            return EnableAttachments(configuration,
                connectionFactory: async () =>
                {
                    var connection = connectionFactory();
                    try
                    {
                        await connection.OpenAsync();
                        return connection;
                    }
                    catch
                    {
                        #if NETSTANDARD2_1
                        await connection.DisposeAsync();
                        #else
                        connection.Dispose();
                        #endif
                        throw;
                    }
                },
                timeToKeep);
        }

        /// <summary>
        /// Enable SQL attachments for this endpoint.
        /// </summary>
        public static AttachmentSettings EnableAttachments(
            this EndpointConfiguration configuration,
            Func<Task<DbConnection>> connectionFactory,
            GetTimeToKeep timeToKeep)
        {
            var settings = configuration.GetSettings();
            AttachmentSettings attachments = new(connectionFactory, timeToKeep);
            return SetAttachments(configuration, settings, attachments);
        }

        static AttachmentSettings SetAttachments(EndpointConfiguration configuration, SettingsHolder settings, AttachmentSettings attachments)
        {
            settings.Set(attachments);
            configuration.EnableFeature<AttachmentFeature>();
            configuration.DisableFeature<AttachmentsUsedWhenNotEnabledFeature>();
            return attachments;
        }
    }
}
