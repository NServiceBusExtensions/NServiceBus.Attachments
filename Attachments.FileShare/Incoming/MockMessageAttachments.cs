using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare.Testing
{
    /// <summary>
    /// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
    /// All members are stubbed out.
    /// </summary>
    /// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
    public class MockMessageAttachments : IMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(string,Stream,CancellationToken)"/>
        /// </summary>
        public virtual Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
        {
            target.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(Stream,CancellationToken)"/>
        /// </summary>
        public virtual Task CopyTo(Stream target, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(string,Func{FileStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(string name, Func<FileStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(Func{FileStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(Func<FileStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreams"/>
        /// </summary>
        public virtual Task ProcessStreams(Func<string, FileStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes(CancellationToken)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytes(CancellationToken cancellation = default)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytes(string name, CancellationToken cancellation = default)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream()"/>
        /// </summary>
        public virtual FileStream GetStream()
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string)"/>
        /// </summary>
        public virtual FileStream GetStream(string name)
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream,CancellationToken)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream,CancellationToken)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{FileStream,Task})"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, string name, Func<FileStream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{FileStream,Task})"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, Func<FileStream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{string, FileStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStreamsForMessage(string messageId, Func<string, FileStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string,string,CancellationToken)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string)"/>
        /// </summary>
        public virtual FileStream GetStreamForMessage(string messageId)
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string)"/>
        /// </summary>
        public virtual FileStream GetStreamForMessage(string messageId, string name)
        {
            return null;
        }
    }
}