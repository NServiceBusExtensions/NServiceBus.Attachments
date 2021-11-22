using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class InstallerTests
{
    static InstallerTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public async Task Run()
    {
        await using var connection = await Connection.OpenAsyncConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        TableExists("[dbo].[MessageAttachments]", connection);
    }

    static void TableExists(string tableName, SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $@"
select case when exists(
    select * from sys.objects where
        object_id = object_id('{tableName}')
        and type in ('U')
) then 1 else 0 end;
";
        var tableExists = (int) command.ExecuteScalar()! == 1;
        Assert.True(tableExists);
    }
}