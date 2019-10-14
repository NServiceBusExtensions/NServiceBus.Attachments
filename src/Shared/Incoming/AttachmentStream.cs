#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#endif
#if Sql
.Sql
#endif
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Wraps a <see cref="Stream"/> to provide extra information when reading.
    /// </summary>
    public class AttachmentStream :
        Stream
    {
        static Dictionary<string, string> emptyDictionary = new Dictionary<string, string>();

        /// <summary>
        /// An empty <see cref="AttachmentStream"/> that contains a "default" name and empty <see cref="MemoryStream"/> as content.
        /// </summary>
        public static AttachmentStream Empty()
        {
            return new AttachmentStream("default", new MemoryStream(), 0, emptyDictionary);
        }

        Stream inner;
        IAsyncDisposable[] cleanups;

        /// <summary>
        /// Initialises a new instance of <see cref="AttachmentStream"/>.
        /// </summary>
        /// <param name="name">The name of the attachment.</param>
        /// <param name="inner">The <see cref="Stream"/> to wrap.</param>
        /// <param name="length">The length of <paramref name="inner"/>.</param>
        /// <param name="metadata">The attachment metadata.</param>
        /// <param name="cleanups">Any extra <see cref="IAsyncDisposable"/>s to cleanup.</param>
        public AttachmentStream(string name, Stream inner, long length, IReadOnlyDictionary<string, string> metadata, params IAsyncDisposable[] cleanups)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(inner, nameof(inner));
            Guard.AgainstNull(metadata, nameof(metadata));
            Guard.AgainstNull(cleanups, nameof(cleanups));
            this.inner = inner;
            this.cleanups = cleanups;
            Name = name;
            Length = length;
            Metadata = metadata;
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

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return inner.ReadAsync(buffer, cancellationToken);
        }

        public override int Read(Span<byte> buffer)
        {
            return inner.Read(buffer);
        }

        public override int ReadByte()
        {
            return inner.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return inner.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return inner.Read(buffer, offset, count);
        }

        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => inner.CanSeek;
        public override bool CanTimeout => inner.CanTimeout;
        public override bool CanWrite => false;

        public string Name { get; }
        public override long Length { get; }
        /// <summary>
        /// The attachment metadata.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Metadata;
        public override int ReadTimeout => inner.ReadTimeout;

        public override long Position
        {
            get => inner.Position;
            set => inner.Position = value;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
        {
            return inner.BeginRead(buffer, offset, count, callback, state);
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

        public override void CopyTo(Stream destination, int bufferSize)
        {
            inner.CopyTo(destination, bufferSize);
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await inner.DisposeAsync();
            if (cleanups != null)
            {
                await Task.WhenAll(cleanups.Select(async x => await x.DisposeAsync()));
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return inner.EndRead(asyncResult);
        }

        public override object InitializeLifetimeService()
        {
            return inner.InitializeLifetimeService();
        }

        public override bool Equals(object? obj)
        {
            return inner.Equals(obj);
        }

        public override int GetHashCode()
        {
            return inner.GetHashCode();
        }

        public override string? ToString()
        {
            return inner.ToString();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public override int WriteTimeout => throw new NotImplementedException();

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
        {
            throw new NotImplementedException();
        }
    }
}