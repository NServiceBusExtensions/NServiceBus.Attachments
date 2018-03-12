using System;
using System.IO;
using System.Threading.Tasks;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class PersisterTests: TestBase
{
    Persister persister;

    public PersisterTests(ITestOutputHelper output) : base(output)
    {
        var fileShare = Path.GetFullPath("attachments/PersisterTests");
        persister = new Persister(fileShare);
        Directory.CreateDirectory(fileShare);
    }

    [Fact]
    public async Task CopyTo()
    {
        persister.DeleteAllRows();
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream());
        var memoryStream = new MemoryStream();
        await persister.CopyTo("theMessageId", "theName", memoryStream);

        memoryStream.Position = 0;
        Assert.Equal(5, memoryStream.GetBuffer()[0]);
    }

    [Fact]
    public async Task GetBytes()
    {
        persister.DeleteAllRows();
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream());
        var bytes = await persister.GetBytes("theMessageId", "theName");
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task ProcessStream()
    {
        persister.DeleteAllRows();
        var count = 0;
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream());
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
        persister.DeleteAllRows();
        var count = 0;
        await persister.SaveStream("theMessageId", "theName1", new DateTime(2000, 1, 1, 1, 1, 1), GetStream(1));
        await persister.SaveStream("theMessageId", "theName2", new DateTime(2000, 1, 1, 1, 1, 1), GetStream(2));
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
    public void SaveStream()
    {
        persister.DeleteAllRows();
        persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
        ObjectApprover.VerifyWithJson(persister.ReadAllMetadata());
    }

    [Fact]
    public void SaveBytes()
    {
        persister.DeleteAllRows();
        persister.SaveBytes("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), new byte[] {1}).GetAwaiter().GetResult();
        ObjectApprover.VerifyWithJson(persister.ReadAllMetadata());
    }

    [Fact]
    public void CleanupItemsOlderThan()
    {
        persister.DeleteAllRows();
        persister.SaveStream("theMessageId1", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
        persister.SaveStream("theMessageId2", "theName", new DateTime(2002, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
        persister.CleanupItemsOlderThan(new DateTime(2001, 1, 1, 1, 1, 1));
        ObjectApprover.VerifyWithJson(persister.ReadAllMetadata());
    }

    Stream GetStream(byte content=5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}