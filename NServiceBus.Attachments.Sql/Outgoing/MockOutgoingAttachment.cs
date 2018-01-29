using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Extensibility;

namespace NServiceBus.Attachments.Testing
{
    public class MockOutgoingAttachment : IOutgoingAttachment
    {
        public IMessageHandlerContext HandlerContext { get; }
        public IMessageSession MessageSession { get; }
        public ExtendableOptions Options { get; }

        public MockOutgoingAttachment()
        {
        }

        public MockOutgoingAttachment(IMessageSession context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            MessageSession = context;
            Options = options;
        }

        public MockOutgoingAttachment(IMessageHandlerContext context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            HandlerContext = context;
            Options = options;
        }

        public virtual void Add<T>(Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
        {
        }

        public virtual void Add(Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        {
        }

        public virtual void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        {
            stream.Dispose();
        }
    }
}