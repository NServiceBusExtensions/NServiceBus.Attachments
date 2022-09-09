using Microsoft.Data.SqlClient;

public static class Connection
{
    public static string ConnectionString;

    static Connection()
    {
        if (Environment.GetEnvironmentVariable("APPVEYOR") == "True")
        {
            ConnectionString = @"Server=(local)\SQL2019;Database=master;User ID=sa;Password=Password12!;TrustServerCertificate=True";
            return;
        }

        var connectionEnvironmentVariable = Environment.GetEnvironmentVariable("attachmentconnection");
        if (connectionEnvironmentVariable is not null)
        {
            ConnectionString = connectionEnvironmentVariable;
            IsUsingEnvironmentVariable = true;
            return;
        }

        ConnectionString = "Data Source=.;Database=NServiceBusAttachmentsTests;Integrated Security=True;Max Pool Size=100;TrustServerCertificate=True";
    }

    public static bool IsUsingEnvironmentVariable;

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static async Task<SqlConnection> OpenAsyncConnection(CancellationToken cancellation = default)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellation);
        return connection;
    }

    public static SqlConnection NewConnection() => new(ConnectionString);
}