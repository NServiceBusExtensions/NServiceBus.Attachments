using System.Collections.Generic;
using System.Data.SqlClient;

public static class SqlHelper
{
    public static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var database = builder.InitialCatalog;

        var masterConnection = connectionString.Replace(builder.InitialCatalog, "master");

        using (var connection = new SqlConnection(masterConnection))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
if(db_id('{database}') is null)
    create database [{database}]
";
                command.ExecuteNonQuery();
            }
        }
    }

    public static IEnumerable<Dictionary<string, string>> ReadAllRows(SqlConnection connection)
    {
        using (var command = connection.CreateCommand())
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                yield return new Dictionary<string, string>
                {
                    ["Id"] = reader["Id"].ToString(),
                    ["MessageId"] = reader["MessageId"].ToString(),
                    ["Name"] = reader["Name"].ToString(),
                    ["Expiry"] = reader["Expiry"].ToString(),
                    ["Data"] = reader["Data"].ToString()
                };
            }
        }
    }
}