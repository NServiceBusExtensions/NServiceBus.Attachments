#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#endif
#if Sql
.Sql
#endif
{
    /// <summary>
    /// Wraps a <see cref="Stream"/> to provide extra information when reading.
    /// </summary>
    public class AttachmentStream : Stream
    {
        Stream inner;
        IDisposable[] cleanups;

        /// <summary>
        /// Initialises a new instance of <see cref="AttachmentStream"/>.
        /// </summary>
        /// <param name="inner">The <see cref="Stream"/> to wrap.</param>
        /// <param name="length">The length of <paramref name="inner"/>.</param>
        /// <param name="metadata">The attachment metadata.</param>
        /// <param name="cleanups">Any extra <see cref="IDisposable"/>s to cleanup.</param>
        public AttachmentStream(Stream inner, long length, IReadOnlyDictionary<string, string> metadata, params IDisposable[] cleanups)
        {
            Guard.AgainstNull(inner, nameof(inner));
            Guard.AgainstNull(metadata, nameof(metadata));
            Guard.AgainstNull(cleanups, nameof(cleanups));
            this.inner = inner;
            this.cleanups = cleanups;
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

        public override long Length { get; }
        public IReadOnlyDictionary<string, string> Metadata;
        public override int ReadTimeout => inner.ReadTimeout;

        public override long Position
        {
            get => inner.Position;
            set => inner.Position = value;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            inner.Dispose();
            if (cleanups != null)
            {
                foreach (var cleanup in cleanups)
                {
                    cleanup.Dispose();
                }
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

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public override int WriteTimeout => throw new NotImplementedException();

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }
    }
}