using NServiceBus.Attachments.Sql;

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
            Installer.CreateTable(sqlConnection, "MessageAttachments").Wait();
        }
    }
}