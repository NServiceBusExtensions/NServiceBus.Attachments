using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using Xunit;

public class InstallerTests
{
    static InstallerTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public async Task Run()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection, "MessageAttachments");
        }

        TableExists("[dbo].[MessageAttachments]");
    }

    static void TableExists(string tableName)
    {
        using (var connection = Connection.OpenConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
select case when exists(
    select * from sys.objects where
        object_id = object_id('{tableName}')
        and type in ('U')
) then 1 else 0 end;
";
            var tableExists = (int) command.ExecuteScalar() == 1;
            Assert.True(tableExists);
        }
    }
}