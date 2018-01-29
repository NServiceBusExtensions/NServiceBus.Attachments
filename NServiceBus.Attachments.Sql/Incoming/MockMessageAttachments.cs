using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Testing
{
    public class MockMessageAttachments : IMessageAttachments
    {
        public IMessageHandlerContext Context { get; }
        public string MessageId { get; }

        public MockMessageAttachments()
        {
        }

        public MockMessageAttachments(IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            Context = context;
        }

        public MockMessageAttachments(IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageId, nameof(messageId));
            Context = context;
            MessageId = messageId;
        }

        public virtual Task CopyTo(string name, Stream target)
        {
            target.Dispose();
            return Task.CompletedTask;
        }

        public virtual Task CopyTo(Stream target)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStream(string name, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStream(Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStreams(Func<string, Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task<byte[]> GetBytes()
        {
            return Task.FromResult(new byte[] { });
        }

        public virtual Task<byte[]> GetBytes(string name)
        {
            return Task.FromResult(new byte[] { });
        }

        public virtual Task<Stream> GetStream()
        {
            return Task.FromResult<Stream>(null);
        }

        public virtual Task<Stream> GetStream(string name)
        {
            return Task.FromResult<Stream>(null);
        }

        public virtual Task CopyToForMessage(string messageId, string name, Stream target)
        {
            return Task.CompletedTask;
        }

        public virtual Task CopyToForMessage(string messageId, Stream target)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task<byte[]> GetBytesForMessage(string messageId)
        {
            return Task.FromResult(new byte[] { });
        }

        public virtual Task<byte[]> GetBytesForMessage(string messageId, string name)
        {
            return Task.FromResult(new byte[] { });
        }

        public virtual Task<Stream> GetStreamForMessage(string messageId)
        {
            return Task.FromResult<Stream>(null);
        }

        public virtual Task<Stream> GetStreamForMessage(string messageId, string name)
        {
            return Task.FromResult<Stream>(null);
        }
    }
}