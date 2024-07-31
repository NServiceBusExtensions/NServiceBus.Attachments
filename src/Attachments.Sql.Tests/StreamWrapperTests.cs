public class StreamWrapperTests
{
    static byte[] buffer = "content"u8.ToArray();

    [Fact]
    public Task ReadBytesAsync() =>
        Run(_ => _.ReadAsync(new byte[2], 0, 2));

    [Fact]
    public void ReadBytes() =>
        Run(_ => _.Read(new byte[2], 0, 2));

    [Fact]
    public void ReadSpan() =>
        Run(_ => _.Read(new(new byte[2])));

    [Fact]
    public Task ReadMemory() =>
        Run(async _ => await _.ReadAsync(new(new byte[2])));

    [Fact]
    public void ReadByte() =>
        Run(_ => _.ReadByte());

    [Fact]
    public void CopyTo() =>
        Run(_ => _.CopyTo(new MemoryStream()));

    [Fact]
    public Task CopyToAsync() =>
        Run(_ => _.CopyToAsync(new MemoryStream()));

    static void Run(Action<AttachmentStream> action)
    {
        using var stream = new MemoryStream(buffer);
        var wrapper = new AttachmentStream("name", stream,buffer.Length, new Dictionary<string, string>());
        action(wrapper);
        Assert.Equal(stream.Position, wrapper.Position);
    }

    static async Task Run(Func<AttachmentStream, Task> action)
    {
        using var stream = new MemoryStream(buffer);
        var wrapper = new AttachmentStream("name", stream,buffer.Length, new Dictionary<string, string>());
        await action(wrapper);
        Assert.Equal(stream.Position, wrapper.Position);
    }
}
