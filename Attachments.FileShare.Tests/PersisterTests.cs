using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class PersisterTests : TestBase
{
    public PersisterTests(ITestOutputHelper output) : base(output)
    {
    }

    static Persister GetPersister([CallerMemberName] string path = null)
    {
        var fileShare = Path.GetFullPath($"attachments/{path}");
        var persister = new Persister(fileShare);
        Directory.CreateDirectory(fileShare);
        persister.DeleteAllRows();
        return persister;
    }

    [Fact]
    public async Task CopyTo()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream());
        var memoryStream = new MemoryStream();
        await persister.CopyTo("theMessageId", "theName", memoryStream);

        memoryStream.Position = 0;
        Assert.Equal(5, memoryStream.GetBuffer()[0]);
    }

    [Fact]
    public async Task GetBytes()
    {
        var persister = GetPersister();
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream());
        var bytes = await persister.GetBytes("theMessageId", "theName");
        Assert.Equal(5, bytes[0]);
    }

    [Fact]
    public async Task ProcessStream()
    {
        var persister = GetPersister();
        var count = 0;
        await persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream());
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
        await persister.SaveStream("theMessageId", "theName1", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream(1));
        await persister.SaveStream("theMessageId", "theName2", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream(2));
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
        var persister = GetPersister();
        persister.SaveStream("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream()).GetAwaiter().GetResult();
        ObjectApprover.VerifyWithJson(persister.ReadAllMetadata());
    }

    [Fact]
    public void SaveBytes()
    {
        var persister = GetPersister();
        persister.SaveBytes("theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), new byte[] {1}).GetAwaiter().GetResult();
        var allMetadata = persister.ReadAllMetadata().ToList();

        ObjectApprover.VerifyWithJson(allMetadata);
    }

    [Fact]
    public void CleanupItemsOlderThan()
    {
        var persister = GetPersister();
        persister.SaveStream("theMessageId1", "theName", new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream()).GetAwaiter().GetResult();
        persister.SaveStream("theMessageId2", "theName", new DateTime(2002, 1, 1, 1, 1, 1, DateTimeKind.Utc), GetStream()).GetAwaiter().GetResult();
        persister.CleanupItemsOlderThan(new DateTime(2001, 1, 1, 1, 1, 1));
        ObjectApprover.VerifyWithJson(persister.ReadAllMetadata());
    }

    Stream GetStream(byte content = 5)
    {
        var stream = new MemoryStream();
        stream.WriteByte(content);
        stream.Position = 0;
        return stream;
    }
}