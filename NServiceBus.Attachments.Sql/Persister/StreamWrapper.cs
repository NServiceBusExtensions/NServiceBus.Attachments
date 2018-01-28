using System.Data.SqlClient;
using System.IO;

class StreamWrapper:Stream
{
    Stream inner;
    SqlCommand command;
    SqlDataReader reader;

    public StreamWrapper(Stream inner, SqlCommand command,SqlDataReader reader)
    {
        this.inner = inner;
        this.command = command;
        this.reader = reader;
    }
    public override void Flush()
    {
        inner.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        inner.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return inner.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
    }

    //TODO: override others
    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanWrite => inner.CanWrite;
    public override long Length => inner.Length;
    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        reader?.Dispose();
        command?.Dispose();
    }
}