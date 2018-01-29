using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Testing
{
    public class MockMessageAttachment : IMessageAttachment
    {
        public IMessageHandlerContext Context { get; }
        public string MessageId { get; }

        public MockMessageAttachment()
        {
        }

        public MockMessageAttachment(IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            Context = context;
        }

        public MockMessageAttachment(IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageId, nameof(messageId));
            Context = context;
            MessageId = messageId;
        }

        public virtual Task CopyTo(Stream target)
        {
            target.Dispose();
            return Task.CompletedTask;
        }

        public virtual Task ProcessStream(Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public virtual Task<byte[]> GetBytes()
        {
            return Task.FromResult(new byte[] { });
        }

        public virtual Task<Stream> GetStream()
        {
            return Task.FromResult<Stream>(null);
        }
    }
}