using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Extensibility;

namespace NServiceBus.Attachments.Testing
{
    public class MockOutgoingAttachments : IOutgoingAttachments
    {
        public IMessageSession MessageSession { get; }
        public IMessageHandlerContext HandlerContext { get; }
        public ExtendableOptions Options { get; }

        public MockOutgoingAttachments()
        {
        }

        public MockOutgoingAttachments(IMessageSession context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            MessageSession = context;
            Options = options;
        }

        public MockOutgoingAttachments(IMessageHandlerContext context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            HandlerContext = context;
            Options = options;
        }

        public void Add<T>(string name, Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
        {
        }

        public void Add(string name, Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        {
        }

        public void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        {
            stream.Dispose();
        }
    }
}