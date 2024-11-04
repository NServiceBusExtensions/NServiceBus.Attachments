using System.Data;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Settings;

namespace NServiceBus;

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
        Func<SqlConnection> connectionFactory,
        GetTimeToKeep? timeToKeep = null)
    {
        var dbConnection = connectionFactory();
        if (dbConnection.State == ConnectionState.Open)
        {
            throw new("This overload of EnableAttachments expects `Func<SqlConnection> connectionFactory` to return a un-opened SqlConnection.");
        }

        return EnableAttachments(configuration,
            connectionFactory: async cancelaltion =>
            {
                var connection = connectionFactory();
                try
                {
                    await connection.OpenAsync(cancelaltion);
                    return connection;
                }
                catch
                {
                    connection.Dispose();
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
        Func<Cancel, Task<SqlConnection>> connectionFactory,
        GetTimeToKeep? timeToKeep = null)
    {
        var settings = configuration.GetSettings();
        var attachments = new AttachmentSettings(connectionFactory, timeToKeep ?? TimeToKeep.Default);
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