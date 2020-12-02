using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

public static class Connection
{
    public static string ConnectionString;

    static Connection()
    {
        if (Environment.GetEnvironmentVariable("APPVEYOR") == "True")
        {
            ConnectionString = @"Server=(local)\SQL2017;Database=master;User ID=sa;Password=Password12!";
            return;
        }

        var connectionEnvironmentVariable = Environment.GetEnvironmentVariable("attachmentconnection");
        if (connectionEnvironmentVariable != null)
        {
            ConnectionString = connectionEnvironmentVariable;
            IsUsingEnvironmentVariable = true;
            return;
        }

        ConnectionString = "Data Source=.;Database=NServiceBusAttachmentsTests; Integrated Security=True;Max Pool Size=100";
    }

    public static bool IsUsingEnvironmentVariable;

    public static DbConnection OpenConnection()
    {
        SqlConnection connection = new(ConnectionString);
        connection.Open();
        return connection;
    }

    public static async Task<DbConnection> OpenAsyncConnection(CancellationToken cancellation = default)
    {
        SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellation);
        return connection;
    }

    public static DbConnection NewConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}