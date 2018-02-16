using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql.Testing
{
    /// <summary>
    /// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
    /// All members are stubbed out.
    /// </summary>
    /// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
    public class MockMessageAttachments : IMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(string,Stream)"/>
        /// </summary>
        public virtual Task CopyTo(string name, Stream target)
        {
            target.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(Stream)"/>
        /// </summary>
        public virtual Task CopyTo(Stream target)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(string,Func{Stream,Task})"/>
        /// </summary>
        public virtual Task ProcessStream(string name, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(Func{Stream,Task})"/>
        /// </summary>
        public virtual Task ProcessStream(Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreams"/>
        /// </summary>
        public virtual Task ProcessStreams(Func<string, Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes()"/>
        /// </summary>
        public virtual Task<byte[]> GetBytes()
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes(string)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytes(string name)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream()"/>
        /// </summary>
        public virtual Task<Stream> GetStream()
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string)"/>
        /// </summary>
        public virtual Task<Stream> GetStream(string name)
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, string name, Stream target)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, Stream target)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{Stream,Task})"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{Stream,Task})"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{string, Stream,Task})"/>
        /// </summary>
        public virtual Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytesForMessage(string messageId)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string,string)"/>
        /// </summary>
        public virtual Task<byte[]> GetBytesForMessage(string messageId, string name)
        {
            return Task.FromResult(new byte[] { });
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string)"/>
        /// </summary>
        public virtual Task<Stream> GetStreamForMessage(string messageId)
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string)"/>
        /// </summary>
        public virtual Task<Stream> GetStreamForMessage(string messageId, string name)
        {
            return Task.FromResult<Stream>(null);
        }
    }
}