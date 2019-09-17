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

        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection, "MessageAttachments").Wait();
        }
    }
}