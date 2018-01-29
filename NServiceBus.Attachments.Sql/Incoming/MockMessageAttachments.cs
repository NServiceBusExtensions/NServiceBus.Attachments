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

        public Task CopyTo(string name, Stream target)
        {
            target.Dispose();
            return Task.CompletedTask;
        }

        public Task ProcessStream(string name, Func<Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public Task ProcessStreams(Func<string, Stream, Task> action)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> GetBytes(string name)
        {
            return Task.FromResult(new byte[] { });
        }

        public Task<Stream> GetStream(string name)
        {
            return Task.FromResult<Stream>(null);
        }
    }
}