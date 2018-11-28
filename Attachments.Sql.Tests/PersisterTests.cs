using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class PersisterTests : TestBase
{
    DateTime defaultTestDate = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    Dictionary<string, string> metadata = new Dictionary<string, string> { { "key", "value" } };
    Persister persister;

    static PersisterTests()
    {
        DbSetup.Setup();
    }

    public PersisterTests(ITestOutputHelper output) : base(output)
    {
        persister = new Persister("MessageAttachments");
    }

    [Fact]
    public async Task CopyTo()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection, "MessageAttachments");
            await persister.DeleteAllAttachments(connection, null);
            await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
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
            await Installer.CreateTable(connection, "MessageAttachments");
            await persister.DeleteAllAttachments(connection, null);
            await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
            byte[] bytes = await persister.GetBytes("theMessageId", "theName", connection, null);
            Assert.Equal(5, bytes[0]);
        }
    }

    [Fact]
    public async Task CaseInsensitiveRead()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection, "MessageAttachments");
            await persister.DeleteAllAttachments(connection, null);
            await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
            byte[] bytes = await persister.GetBytes("themeSsageid", "Thename", connection, null);
            Assert.Equal(5, bytes[0]);
        }
    }

    [Fact]
    public async Task ProcessStream()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection, "MessageAttachments");
            await persister.DeleteAllAttachments(connection, null);
            var count = 0;
            await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
            await persister.ProcessStream("theMessageId", "theName", connection, null,
                action: stream =>
                {
                    count++;
                    var array = ToBytes(stream);
                    Assert.Equal(5, array[0]);
                    return Task.CompletedTask;
                });
            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task ProcessStreams()
    {
        using (var connection = Connection.OpenConnection())
        {
            await Installer.CreateTable(connection, "MessageAttachments");
            await persister.DeleteAllAttachments(connection, null);
            var count = 0;
            await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
            await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
            await persister.ProcessStreams("theMessageId", connection, null,
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
            Installer.CreateTable(connection, "MessageAttachments").Wait();
            persister.DeleteAllAttachments(connection, null).Wait();
            persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata).GetAwaiter().GetResult();
            var result = persister.ReadAllInfo(connection, null).GetAwaiter().GetResult();
            Assert.NotNull(result);
            ObjectApprover.VerifyWithJson(result);
        }
    }

    [Fact]
    public void SaveBytes()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection, "MessageAttachments").Wait();
            persister.DeleteAllAttachments(connection, null).Wait();
            persister.SaveBytes(connection, null, "theMessageId", "theName", defaultTestDate, new byte[] {1}, metadata).GetAwaiter().GetResult();
            var result = persister.ReadAllInfo(connection, null).GetAwaiter().GetResult();
            Assert.NotNull(result);
            ObjectApprover.VerifyWithJson(result);
        }
    }

    [Fact]
    public void ReadAllMessageInfo()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection, "MessageAttachments").Wait();
            persister.DeleteAllAttachments(connection, null).Wait();
            persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata).GetAwaiter().GetResult();
            persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata).GetAwaiter().GetResult();
            var list = new List<AttachmentInfo>();
            persister.ReadAllMessageInfo(connection, null, "theMessageId",
                info =>
                {
                    list.Add(info);
                    return Task.CompletedTask;
                }).GetAwaiter().GetResult();
            Assert.NotNull(list);
            ObjectApprover.VerifyWithJson(list);
        }
    }

    [Fact]
    public void CleanupItemsOlderThan()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection, "MessageAttachments").Wait();
            persister.DeleteAllAttachments(connection, null).Wait();
            persister.SaveStream(connection, null, "theMessageId1", "theName", defaultTestDate, GetStream()).GetAwaiter().GetResult();
            persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate.AddYears(2), GetStream()).GetAwaiter().GetResult();
            persister.CleanupItemsOlderThan(connection, null, new DateTime(2001, 1, 1, 1, 1, 1)).Wait();
            var result = persister.ReadAllInfo(connection, null).GetAwaiter().GetResult();
            Assert.NotNull(result);
            ObjectApprover.VerifyWithJson(result);
        }
    }

    Stream GetStream(byte content = 5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}