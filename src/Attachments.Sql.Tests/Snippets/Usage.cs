using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Sql;

public class Usage
{
    string connectionString = null!;

    Usage(EndpointConfiguration configuration)
    {
        #region EnableAttachments

        configuration.EnableAttachments(
            connectionFactory: async () =>
            {
                SqlConnection connection = new(connectionString);
                try
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    return connection;
                }
                catch
                {
                    connection.Dispose();
                    throw;
                }
            },
            timeToKeep: _ => TimeSpan.FromDays(7));

        #endregion

        #region EnableAttachmentsRecommended

        configuration.EnableAttachments(
            connectionFactory: OpenConnection,
            timeToKeep: TimeToKeep.Default);

        #endregion
    }

    void DisableCleanupTask(EndpointConfiguration configuration)
    {
        #region DisableCleanupTask

        var attachments = configuration.EnableAttachments(
            connectionFactory: OpenConnection,
            timeToKeep: TimeToKeep.Default);
        attachments.DisableCleanupTask();

        #endregion
    }

    void UseTransportConnectivity(EndpointConfiguration configuration)
    {
        #region UseTransportConnectivity

        var attachments = configuration.EnableAttachments(
            OpenConnection,
            TimeToKeep.Default);
        attachments.UseTransportConnectivity();

        #endregion
    }

    void UseSynchronizedStorageSessionConnectivity(EndpointConfiguration configuration)
    {
        #region UseSynchronizedStorageSessionConnectivity

        var attachments = configuration.EnableAttachments(
            OpenConnection,
            TimeToKeep.Default);
        attachments.UseSynchronizedStorageSessionConnectivity();

        #endregion
    }

    void ExecuteAtStartup(EndpointConfiguration configuration)
    {
        #region ExecuteAtStartup

        configuration.EnableInstallers();
        var attachments = configuration.EnableAttachments(
            connectionFactory: OpenConnection,
            timeToKeep: TimeToKeep.Default);

        #endregion
    }

    void DisableInstaller(EndpointConfiguration configuration)
    {
        #region DisableInstaller

        configuration.EnableInstallers();
        var attachments = configuration.EnableAttachments(
            connectionFactory: OpenConnection,
            timeToKeep: TimeToKeep.Default);
        attachments.DisableInstaller();

        #endregion
    }

    void UseTableName(EndpointConfiguration configuration)
    {
        #region UseTableName

        var attachments = configuration.EnableAttachments(
            connectionFactory: OpenConnection,
            timeToKeep: TimeToKeep.Default);
        attachments.UseTable(new("CustomAttachmentsTableName", "dbo"));

        #endregion
    }

    #region OpenConnection

    async Task<DbConnection> OpenConnection()
    {
        SqlConnection connection = new(connectionString);
        try
        {
            await connection.OpenAsync().ConfigureAwait(false);
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    #endregion
}