using System;
using System.Data.SqlClient;

public static class Connection
{
    public static string ConnectionString;

    static Connection()
    {
        if (Environment.GetEnvironmentVariable("APPVEYOR")=="True")
        {
            ConnectionString = @"Server=(local)\SQL2017;Database=master;User ID=sa;Password=Password12!";
            return;
        }
        ConnectionString = @"Data Source=.\SQLExpress;Database=NServiceBusAttachmentsTests; Integrated Security=True;Max Pool Size=100";
    }

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static SqlConnection NewConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}