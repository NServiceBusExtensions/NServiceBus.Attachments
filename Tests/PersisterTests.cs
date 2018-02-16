using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class PersisterTests: TestBase
{
    Persister persister;

    static PersisterTests()
    {
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }
    }

    public PersisterTests(ITestOutputHelper output) : base(output)
    {
        persister = new Persister("dbo", "Attachments");
    }

    [Fact]
    public async Task CopyTo()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection);
            await persister.DeleteAllRows(connection, null);
            await persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000,1,1,1,1,1), GetStream());
            var memoryStream = new MemoryStream();
            await persister.CopyTo("theMessageId", "theName", connection, null, memoryStream);

            memoryStream.Position = 0;
            Assert.Equal(5, memoryStream.GetBuffer()[0]);
        }
    }

    [Fact]
    public async Task GetBytes()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection);
            await persister.DeleteAllRows(connection, null);
            await persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream());
            var bytes = await persister.GetBytes("theMessageId", "theName", connection, null);
            Assert.Equal(5, bytes[0]);
        }
    }

    [Fact]
    public async Task ProcessStream()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection);
            await persister.DeleteAllRows(connection, null);
            var count = 0;
            await persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000,1,1,1,1,1), GetStream());
            await persister.ProcessStream("theMessageId", "theName", connection, null,
                action: stream =>
                {
                    count++;
                    var array = ToBytes(stream);
                    Assert.Equal(5, array[0]);
                    return Task.CompletedTask;
                });
            Assert.Equal(1,count);
        }
    }

    [Fact]
    public async Task ProcessStreams()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection);
            await persister.DeleteAllRows(connection, null);
            var count = 0;
            await persister.SaveStream(connection, null, "theMessageId", "theName1", new DateTime(2000, 1, 1, 1, 1, 1), GetStream(1));
            await persister.SaveStream(connection, null, "theMessageId", "theName2", new DateTime(2000, 1, 1, 1, 1, 1), GetStream(2));
            await persister.ProcessStreams("theMessageId", connection,null,
                action: (name, stream) =>
                {
                    count++;
                    var array = ToBytes(stream);
                    if (count == 1)
                    {
                        Assert.Equal(1, array[0]);
                        Assert.Equal("theName1", name);
                    }
                    if (count == 2)
                    {
                        Assert.Equal(2, array[0]);
                        Assert.Equal("theName2", name);
                    }

                    return Task.CompletedTask;
                });
            Assert.Equal(2, count);
        }
    }

    static byte[] ToBytes(Stream stream)
    {
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    [Fact]
    public void SaveStream()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection).Wait();
            persister.DeleteAllRows(connection,null).Wait();
            persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            ObjectApprover.VerifyWithJson(persister.ReadAllMetadata(connection, null));
        }
    }

    [Fact]
    public void SaveBytes()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection).Wait();
            persister.DeleteAllRows(connection,null).Wait();
            persister.SaveBytes(connection, null, "theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), new byte[]{1}).GetAwaiter().GetResult();
            ObjectApprover.VerifyWithJson(persister.ReadAllMetadata(connection, null));
        }
    }

    [Fact]
    public void CleanupItemsOlderThan()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection).Wait();
            persister.DeleteAllRows(connection,null).Wait();
            persister.SaveStream(connection, null, "theMessageId1", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            persister.SaveStream(connection, null, "theMessageId2", "theName", new DateTime(2002, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            persister.CleanupItemsOlderThan(connection,null, new DateTime(2001, 1, 1, 1, 1, 1)).Wait();
            ObjectApprover.VerifyWithJson(persister.ReadAllMetadata(connection,null));
        }
    }

    Stream GetStream(byte content=5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}