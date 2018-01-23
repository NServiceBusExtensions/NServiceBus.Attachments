using System;
using System.Data.SqlClient;

class AttachmentSettings
{
    public readonly Func<SqlConnection> ConnectionBuilder;
    public readonly bool RunCleanTask;
    public readonly string Schema;
    public readonly string TableName;

    public AttachmentSettings(Func<SqlConnection> connectionBuilder, bool runCleanTask, string schema, string tableName)
    {
        ConnectionBuilder = connectionBuilder;
        RunCleanTask = runCleanTask;
        Schema = schema;
        TableName = tableName;
    }
}