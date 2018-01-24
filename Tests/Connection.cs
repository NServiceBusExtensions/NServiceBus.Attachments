using System.Data.SqlClient;

public static class Connection
{
    public const string ConnectionString = @"Data Source=.\SQLExpress;Database=NServiceBusAttachmentsTests; Integrated Security=True;Max Pool Size=100";

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