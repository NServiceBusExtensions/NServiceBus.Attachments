using NServiceBus.Attachments;

public class DbSetup
{
    static bool init;
    public static void Setup()
    {
        if (init)
        {
            return;
        }

        init = true;
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }

        using (var sqlConnection = Connection.OpenConnection())
        {
            Installer.CreateTable(sqlConnection).Wait();
        }
    }
}