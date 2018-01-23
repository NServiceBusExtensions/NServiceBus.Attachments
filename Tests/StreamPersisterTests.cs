using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class StreamPersisterTests: TestBase
{
    StreamPersister streamPersister;

    static StreamPersisterTests()
    {
        SqlHelper.EnsureDatabaseExists(connectionString);
    }

    public StreamPersisterTests(ITestOutputHelper output) : base(output)
    {
        streamPersister = new StreamPersister("dbo", "Attachments");
    }
    const string connectionString = @"Data Source=.\SQLExpress;Database=NServiceBusAttachmentsTests; Integrated Security=True;Max Pool Size=100";

    [Fact]
    public async Task RoundTrip()
    {
        using (var connection = BuildSqlConnection())
        {
            streamPersister.DeleteAllRows(connection);
            Installer.CreateTable(connection);
            await streamPersister.SaveStream(connection, "theMessageId", "theName", new DateTime(2000,1,1,1,1,1), GetStream());
            var memoryStream = new MemoryStream();
            await streamPersister.CopyTo("theMessageId", "theName", connection, memoryStream);

            memoryStream.Position = 0;
            Assert.Equal(5, memoryStream.GetBuffer()[0]);
        }
    }
    [Fact]
    public void SaveStream()
    {
        SqlHelper.EnsureDatabaseExists(connectionString);
        using (var connection = BuildSqlConnection())
        {
            streamPersister.DeleteAllRows(connection);
            Installer.CreateTable(connection);
            streamPersister.SaveStream(connection, "theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            ObjectApprover.VerifyWithJson(streamPersister.ReadAllRows(connection));
        }
    }
    [Fact]
    public void CleanupItemsOlderThan()
    {
        SqlHelper.EnsureDatabaseExists(connectionString);
        using (var connection = BuildSqlConnection())
        {
            streamPersister.DeleteAllRows(connection);
            Installer.CreateTable(connection);
            streamPersister.SaveStream(connection, "theMessageId1", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            streamPersister.SaveStream(connection, "theMessageId2", "theName", new DateTime(2002, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            streamPersister.CleanupItemsOlderThan(connection, new DateTime(2001, 1, 1, 1, 1, 1));
            ObjectApprover.VerifyWithJson(streamPersister.ReadAllRows(connection));
        }
    }

    Stream GetStream()
    {
        var stream = new MemoryStream();
        stream.WriteByte(5);
        stream.Position = 0;
        return stream;
    }

    static SqlConnection BuildSqlConnection()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}