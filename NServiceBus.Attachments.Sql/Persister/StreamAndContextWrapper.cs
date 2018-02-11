using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class StreamAndContextWrapper : Stream
{
    Stream inner;
    SqlCommand command;
    SqlDataReader reader;

    public StreamAndContextWrapper(Stream inner, SqlCommand command, SqlDataReader reader, long length)
    {
        this.inner = inner;
        this.command = command;
        this.reader = reader;
        Length = length;
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        inner.EndWrite(asyncResult);
    }

    public override void Flush()
    {
        inner.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellation)
    {
        return inner.FlushAsync(cancellation);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellation)
    {
        return inner.ReadAsync(buffer, offset, count, cancellation);
    }

    public override int ReadByte()
    {
        return inner.ReadByte();
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

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellation)
    {
        return inner.WriteAsync(buffer, offset, count, cancellation);
    }

    public override void WriteByte(byte value)
    {
        inner.WriteByte(value);
    }

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanTimeout => inner.CanTimeout;
    public override bool CanWrite => inner.CanWrite;
    public override long Length { get; }
    public override int ReadTimeout => inner.ReadTimeout;
    public override int WriteTimeout => inner.WriteTimeout;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return inner.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return inner.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close()
    {
        inner.Close();
        base.Close();
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellation)
    {
        return inner.CopyToAsync(destination, bufferSize, cancellation);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        inner.Dispose();
        reader?.Dispose();
        command?.Dispose();
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return inner.EndRead(asyncResult);
    }

    public override object InitializeLifetimeService()
    {
        return inner.InitializeLifetimeService();
    }

    public override bool Equals(object obj)
    {
        return inner.Equals(obj);
    }

    public override int GetHashCode()
    {
        return inner.GetHashCode();
    }

    public override string ToString()
    {
        return inner.ToString();
    }
}