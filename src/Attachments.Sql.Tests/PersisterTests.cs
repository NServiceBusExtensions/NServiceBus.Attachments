using NServiceBus.Attachments.Sql;

[UsesVerify]
public class PersisterTests
{
    DateTime defaultTestDate = new(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    Dictionary<string, string> metadata = new() {{"key", "value"}};
    Persister persister;

    static PersisterTests()
    {
        DbSetup.Setup();
    }

    public PersisterTests()
    {
        persister = new("MessageAttachments");
    }

    [Fact]
    public async Task CopyTo()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
        MemoryStream memoryStream = new();
        await persister.CopyTo("theMessageId", "theName", connection, null, memoryStream);

        memoryStream.Position = 0;
        Assert.Equal(5, memoryStream.GetBuffer()[0]);
    }

    [Fact]
    public async Task GetBytes()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
        byte[] bytes = await persister.GetBytes("theMessageId", "theName", connection, null);
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task CaseInsensitiveRead()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream());
        byte[] bytes = await persister.GetBytes("themeSsageid", "Thename", connection, null);
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task LongName()
    {
        await using var connection = Connection.OpenConnection();
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
        await using var connection = Connection.OpenConnection();
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
        await using var connection = Connection.OpenConnection();
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
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
        await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
        await foreach (var attachment in persister.GetStreams("theMessageId", connection, null))
        {
            var array = ToBytes(attachment);
            Assert.True(attachment.Name == "theName1" || attachment.Name == "theName2");
            Assert.True(array[0] == 1 || array[0] == 2);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetMultipleBytes()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveStream(connection, null, "theMessageId", "theName1", defaultTestDate, GetStream(1), metadata);
        await persister.SaveStream(connection, null, "theMessageId", "theName2", defaultTestDate, GetStream(2), metadata);
        await foreach (var attachment in persister.GetBytes("theMessageId", connection, null))
        {
            Assert.True(attachment.Name == "theName1" || attachment.Name == "theName2");
            Assert.True(attachment.Bytes[0] == 1 || attachment.Bytes[0] == 2);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetMultipleStrings()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var count = 0;
        await persister.SaveString(connection, null, "theMessageId", "theName1", defaultTestDate, "a", null, metadata);
        await persister.SaveString(connection, null, "theMessageId", "theName2", defaultTestDate, "b", null, metadata);
        await foreach (var attachment in persister.GetStrings("theMessageId", connection, null))
        {
            Assert.True(attachment.Name == "theName1" || attachment.Name == "theName2");
            Assert.True(attachment.Value == "a" || attachment.Value == "b", attachment.Value);
            Interlocked.Increment(ref count);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ProcessStreams()
    {
        await using var connection = Connection.OpenConnection();
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
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId", "theName", defaultTestDate, GetStream(), metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task SaveBytes()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName", defaultTestDate, new byte[] {1}, metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task SaveString()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveString(connection, null, "theMessageId", "theName", defaultTestDate, "foo", null, metadata);
        var result = persister.ReadAllInfo(connection, null);
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task SaveStringEncoding()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        var expected = "¡™£¢∞§¶•ªº–≠";
        UTF8Encoding encoding = new(true);
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
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        var names = await persister.Duplicate("theSourceMessageId", connection, null, "theTargetMessageId");
        var allInfo = await persister.ReadAllInfo(connection, null);
        await Verifier.Verify(new {names, allInfo});
    }

    [Fact]
    public async Task Duplicate()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", connection, null, "theTargetMessageId");
        var result = persister.ReadAllInfo(connection, null);
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task DuplicateWithRename()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theSourceMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.Duplicate("theSourceMessageId", "theName1", connection, null, "theTargetMessageId", "theName2");
        var result = persister.ReadAllInfo(connection, null);
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task ReadAllMessageInfoAction()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        List<AttachmentInfo> list = new();
        await persister.ReadAllMessageInfo(connection, null, "theMessageId",
            info =>
            {
                list.Add(info);
                return Task.CompletedTask;
            });
        await Verifier.Verify(list);
    }

    [Fact]
    public async Task ReadAllMessageInfo()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await Verifier.Verify(persister.ReadAllMessageInfo(connection, null, "theMessageId"));
    }

    [Fact]
    public async Task ReadAllMessageNames()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveBytes(connection, null, "theMessageId", "theName1", defaultTestDate, new byte[] {1}, metadata);
        await persister.SaveBytes(connection, null, "theMessageId", "theName2", defaultTestDate, new byte[] {1}, metadata);
        await Verifier.Verify(persister.ReadAllMessageNames(connection, null, "theMessageId"));
    }

    [Fact]
    public async Task CleanupItemsOlderThan()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate.AddYears(2), GetStream());
        var cleanupCount = await persister.CleanupItemsOlderThan(connection, null, new(2001, 1, 1, 1, 1, 1));
        var result = await persister.ReadAllInfo(connection, null);
        await Verifier.Verify(new {cleanupCount, result});
    }

    [Fact]
    public async Task PurgeItems()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName1", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId1", "theName2", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate, GetStream());
        var purgeCount = await persister.PurgeItems(connection, null);
        var result = await persister.ReadAllInfo(connection, null);
        await Verifier.Verify(
            new
            {
                result,
                purgeCount
            });
    }

    [Fact]
    public async Task DeleteAttachments()
    {
        await using var connection = Connection.OpenConnection();
        await Installer.CreateTable(connection, "MessageAttachments");
        await persister.DeleteAllAttachments(connection, null);
        await persister.SaveStream(connection, null, "theMessageId1", "theName1", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId1", "theName2", defaultTestDate, GetStream());
        await persister.SaveStream(connection, null, "theMessageId2", "theName", defaultTestDate, GetStream());
        var deleteCount = await persister.DeleteAttachments("theMessageId1", connection, null);
        var result = await persister.ReadAllInfo(connection, null);
        await Verifier.Verify(
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