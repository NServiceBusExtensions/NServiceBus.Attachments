using System;
using System.Data.SqlClient;
using NServiceBus.Attachments;

class AttachmentSettings
{
    public readonly Func<SqlConnection> ConnectionBuilder;
    public readonly bool RunCleanTask;
    public readonly string Schema;
    public readonly string TableName;
    public readonly bool DisableInstaller;
    public readonly GetTimeToKeep TimeToKeep;

    public AttachmentSettings(Func<SqlConnection> connectionBuilder, bool runCleanTask, string schema, string tableName, bool disableInstaller, GetTimeToKeep timeToKeep)
    {
        ConnectionBuilder = connectionBuilder;
        RunCleanTask = runCleanTask;
        Schema = schema;
        TableName = tableName;
        DisableInstaller = disableInstaller;
        TimeToKeep = timeToKeep;
    }
}