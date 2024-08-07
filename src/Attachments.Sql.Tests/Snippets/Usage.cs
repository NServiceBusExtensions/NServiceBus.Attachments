﻿using Microsoft.Data.SqlClient;

// ReSharper disable UnusedVariable
// ReSharper disable RedundantArgumentDefaultValue

public class Usage
{
    string connectionString = null!;

    Usage(EndpointConfiguration configuration)
    {
        #region EnableAttachments

        configuration.EnableAttachments(
            connectionFactory: async cancel =>
            {
                var connection = new SqlConnection(connectionString);
                try
                {
                    await connection.OpenAsync(cancel).ConfigureAwait(false);
                    return connection;
                }
                catch
                {
                    await connection.DisposeAsync();
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

    async Task<SqlConnection> OpenConnection(Cancel cancel)
    {
        var connection = new SqlConnection(connectionString);
        try
        {
            await connection.OpenAsync(cancel).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    #endregion
}