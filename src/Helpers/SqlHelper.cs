using Microsoft.Data.SqlClient;

public static class SqlHelper
{
    public static void EnsureDatabaseExists(string connectionString)
    {
        SqlConnectionStringBuilder builder = new(connectionString);
        var database = builder.InitialCatalog;

        var masterConnection = connectionString.Replace(builder.InitialCatalog, "master");

        using SqlConnection connection = new(masterConnection);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $@"
if(db_id('{database}') is null)
    create database [{database}]
";
        command.ExecuteNonQuery();
    }
}