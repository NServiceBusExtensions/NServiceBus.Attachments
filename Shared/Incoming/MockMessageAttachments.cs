using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable PartialTypeWithSinglePart

namespace NServiceBus.Attachments
#if FileShare
    .FileShare.Testing
#endif
#if Sql
.Sql.Testing
#endif
{
    /// <summary>
    /// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
    /// All members are stubbed out.
    /// </summary>
    /// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
    public partial class MockMessageAttachments : IMessageAttachments
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
        /// <see cref="IMessageAttachments.ProcessStream(string,Func{AttachmentStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreams"/>
        /// </summary>
        public virtual Task ProcessStreams(Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
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
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{AttachmentStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{AttachmentStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{string, AttachmentStream,Task},CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStreamsForMessage(string messageId, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
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
    }
}