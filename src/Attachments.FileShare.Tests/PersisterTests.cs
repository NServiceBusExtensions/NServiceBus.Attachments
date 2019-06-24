using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NServiceBus.Attachments.FileShare;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class PersisterTests : TestBase
{
    DateTime defaultTestDate = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    Dictionary<string, string> metadata = new Dictionary<string, string> { { "key", "value" } };

    public PersisterTests(ITestOutputHelper output) : base(output)
    {
    }

    static Persister GetPersister([CallerMemberName] string path = null)
    {
        var fileShare = Path.GetFullPath($"attachments/{path}");
        var persister = new Persister(fileShare);
        Directory.CreateDirectory(fileShare);
        persister.DeleteAllAttachments();
        return persister;
    }

    [Fact]
    public async Task CopyTo()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName", defaultTestDate, GetStream());
        var memoryStream = new MemoryStream();
        await persister.CopyTo("theMessageId", "theName", memoryStream);

        memoryStream.Position = 0;
        Assert.Equal(5, memoryStream.GetBuffer()[0]);
    }

    [Fact]
    public async Task GetBytes()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName", defaultTestDate,GetStream(), metadata);
        byte[] bytes = await persister.GetBytes("theMessageId", "theName");
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task CaseInsensitiveRead()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName", defaultTestDate, GetStream());
        byte[] bytes = await persister.GetBytes("themeSsageid", "Thename");
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task ProcessStream()
    {
        var persister = GetPersister();
        var count = 0;
        await persister.SaveStream("theMessageId", "theName", defaultTestDate, GetStream());
        await persister.ProcessStream("theMessageId", "theName",
            action: stream =>
            {
                count++;
                var array = ToBytes(stream);
                Assert.Equal(5, array[0]);
                return Task.CompletedTask;
            });
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task ProcessStreams()
    {
        var persister = GetPersister();
        var count = 0;
        await persister.SaveStream("theMessageId", "theName1", defaultTestDate, GetStream(1));
        await persister.SaveStream("theMessageId", "theName2", defaultTestDate, GetStream(2));
        await persister.ProcessStreams("theMessageId",
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

    static byte[] ToBytes(Stream stream)
    {
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    [Fact]
    public async Task SaveStream()
    {
        var persister = GetPersister();
        await persister.SaveStream(
            messageId: "theMessageId", "theName",
            expiry: defaultTestDate,
            stream: GetStream(),
            metadata: metadata);
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    [Fact]
    public async Task ReadAllMessageInfo()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName1", defaultTestDate, GetStream(), metadata);
        await persister.SaveStream("theMessageId", "theName2", defaultTestDate, GetStream(), metadata);
        ObjectApprover.VerifyWithJson(persister.ReadAllMessageInfo("theMessageId"));
    }

    [Fact]
    public async Task SaveBytes()
    {
        var persister = GetPersister();
        await persister.SaveBytes("theMessageId", "theName", defaultTestDate, new byte[] {1}, metadata);
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    [Fact]
    public async Task DuplicateWithRename()
    {
        var persister = GetPersister();
        await persister.SaveStream("theSourceMessageId", "theName1", defaultTestDate, GetStream(), metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", "theTargetMessageId", "theName2");
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    [Fact]
    public async Task Duplicate()
    {
        var persister = GetPersister();
        await persister.SaveStream("theSourceMessageId", "theName1", defaultTestDate, GetStream(), metadata);
        await persister.SaveStream("theSourceMessageId", "theName2", defaultTestDate, GetStream(), metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", "theTargetMessageId");
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    [Fact]
    public async Task DuplicateAll()
    {
        var persister = GetPersister();
        await persister.SaveStream("theSourceMessageId", "theName1", defaultTestDate, GetStream(), metadata);
        await persister.SaveStream("theSourceMessageId", "theName2", defaultTestDate, GetStream(), metadata);
        await persister.Duplicate("theSourceMessageId", "theTargetMessageId");
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    [Fact]
    public async Task CleanupItemsOlderThan()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId1", "theName", defaultTestDate, GetStream());
        await persister.SaveStream("theMessageId2", "theName", defaultTestDate.AddYears(2), GetStream());
        persister.CleanupItemsOlderThan(new DateTime(2001, 1, 1, 1, 1, 1));
        ObjectApprover.VerifyWithJson(persister.ReadAllInfo());
    }

    Stream GetStream(byte content = 5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}