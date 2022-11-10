using NServiceBus.Attachments.Sql;

[UsesVerify]
public class PersisterTests
{
    DateTime defaultTestDate = new(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    Dictionary<string, string> metadata = new()
        {{"key", "value"}};
    Persister persister;

    static PersisterTests() => DbSetup.Setup();

    public PersisterTests() => persister = new("MessageAttachments");

    [Fact]
    public async Task CopyTo()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
        var memoryStream = new MemoryStream();
        await persister.CopyTo("theMessageId", "theName", connection, null, memoryStream);

        memoryStream.Position = 0;
        Assert.Equal(5, memoryStream.GetBuffer()[0]);
    }

    [Fact]
    public async Task GetBytes()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
        byte[] bytes = await persister.GetBytes("theMessageId", "theName", connection, null);
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task GetMemoryStream()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
        var bytes = await persister.GetMemoryStream("theMessageId", "theName", connection, null);
        Assert.Equal(5, bytes.ReadByte());
    }

    [Fact]
    public async Task CaseInsensitiveRead()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
        byte[] bytes = await persister.GetBytes("themeSsageid", "Thename", connection, null);
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task LongName()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var name = new string('a', 255);
        await persister.SaveStream(connection, null, "theMessageId", name, defaultTestDate, GetStream());
        byte[] bytes = await persister.GetBytes("theMessageId", name, connection, null);
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task ProcessStream()
    {
        using var connection = Connection.OpenConnection();
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

    [Fact]
    public async Task ProcessStreamMultiple()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        for (var i = 0; i < 10; i++)
        {
            await persister.SaveStream(connection, null, "theMessageId", $"theName{i}", defaultTestDate, GetStream(), metadata);
        }

        for (var i = 0; i < 10; i++)
        {
            await persister.ProcessStream("theMessageId", $"theName{i}", connection, null,
                action: stream =>
                {
                    Interlocked.Increment(ref count);
                    var array = ToBytes(stream);
                    Assert.Equal(5, array[0]);
                    return Task.CompletedTask;
                });
        }

        Assert.Equal(10, count);
    }

    [Fact]
    public async Task GetMultipleStreams()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
        await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
        await foreach (var attachment in persister.GetStreams("theMessageId", connection, null))
        {
            var array = ToBytes(attachment);
            Assert.True(attachment.Name is "theName1" or "theName2");
            Assert.True(array[0] == 1 || array[0] == 2);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetMultipleBytes()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
        await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
        await foreach (var attachment in persister.GetBytes("theMessageId", connection, null))
        {
            Assert.True(attachment.Name is "theName1" or "theName2");
            Assert.True(attachment.Bytes[0] == 1 || attachment.Bytes[0] == 2);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetMultipleStrings()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveString(connection, null, "theMessageId", "theName1", defaultTestDate, "a", null, metadata);
        await persister.SaveString(connection, null, "theMessageId", "theName2", defaultTestDate, "b", null, metadata);
        await foreach (var attachment in persister.GetStrings("theMessageId", connection, null))
        {
            Assert.True(attachment.Name is "theName1" or "theName2");
            Assert.True(attachment.Value is "a" or "b", attachment.Value);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ProcessStreams()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
        await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
        await persister.ProcessStreams("theMessageId", connection, null,
            action: stream =>
            {
                count++;
                var array = ToBytes(stream);
                if (count == 1)
                {
                    Assert.Equal(1, array[0]);
                    Assert.Equal("theName1", stream.Name);
                }

                if (count == 2)
                {
                    Assert.Equal(2, array[0]);
                    Assert.Equal("theName2", stream.Name);
                }

                return Task.CompletedTask;
            });
        Assert.Equal(2, count);
    }

    static byte[] ToBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    [Fact]
    public async Task SaveStream()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verify(result);
    }

    [Fact]
    public async Task SaveBytes()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName", defaultTestDate, new byte[] {1}, metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verify(result);
    }

    [Fact]
    public async Task SaveString()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveString(connection, null, "theMessageId", "theName", defaultTestDate, "foo", null, metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verify(result);
    }

    [Fact]
    public async Task LargeString()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var expected = new string('*', 100000);
        await persister.SaveString(connection, null, "theMessageId", "theName", defaultTestDate, expected, null, metadata);
        var result = await persister.GetString("theMessageId", "theName", connection, null);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SaveStringEncoding()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var expected = "¡™£¢∞§¶•ªº–≠";
        var encoding = new UTF8Encoding(true);
        await persister.SaveString(connection, null, "theMessageId", "theName", defaultTestDate, expected, encoding, metadata);
        var result = await persister.GetString("theMessageId", "theName", connection, null, encoding);
        Trace.Write(result);
        var attachmentBytes = await persister.GetBytes("theMessageId", "theName", connection, null);
        var bytes = attachmentBytes.Bytes;
        Assert.True(bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF, "Expected a BOM");
        Assert.Equal(expected.ToBytes(encoding), bytes);
    }

    [Fact]
    public async Task DuplicateAll()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        var names = await persister.Duplicate("theSourceMessageId", connection, null, "theTargetMessageId");
        var allInfo = await persister.ReadAllInfo(connection, null);
        await Verify(new {names, allInfo});
    }

    [Fact]
    public async Task Duplicate()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", connection, null, "theTargetMessageId");
        var result = persister.ReadAllInfo(connection, null);
        await Verify(result);
    }

    [Fact]
    public async Task DuplicateWithRename()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", connection, null, "theTargetMessageId", "theName2");
        var result = persister.ReadAllInfo(connection, null);
        await Verify(result);
    }

    [Fact]
    public async Task ReadAllMessageInfoAction()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        var list = new List<AttachmentInfo>();
        await persister.ReadAllMessageInfo(connection, null, "theMessageId",
            info =>
            {
                list.Add(info);
                return Task.CompletedTask;
            });
        await Verify(list);
    }

    [Fact]
    public async Task ReadAllMessageInfo()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await Verify(persister.ReadAllMessageInfo(connection, null, "theMessageId"));
    }

    [Fact]
    public async Task ReadAllMessageNames()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await Verify(persister.ReadAllMessageNames(connection, null, "theMessageId"));
    }

    [Fact]
    public async Task CleanupItemsOlderThan()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate.AddYears(2), GetStream());
        var cleanupCount = await persister.CleanupItemsOlderThan(connection, null, new(2001, 1, 1, 1, 1, 1));
        var result = await persister.ReadAllInfo(connection, null);
        await Verify(new {cleanupCount, result});
    }

    [Fact]
    public async Task PurgeItems()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName1", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId1", "theName2", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate, GetStream());
        var purgeCount = await persister.PurgeItems(connection, null);
        var result = await persister.ReadAllInfo(connection, null);
        await Verify(
            new
            {
                result,
                purgeCount
            });
    }

    [Fact]
    public async Task DeleteAttachments()
    {
        using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName1", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId1", "theName2", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate, GetStream());
        var deleteCount = await persister.DeleteAttachments("theMessageId1", connection, null);
        var result = await persister.ReadAllInfo(connection, null);
        await Verify(
            new
            {
                result,
                deleteCount
            });
    }

    static Stream GetStream(byte content = 5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}