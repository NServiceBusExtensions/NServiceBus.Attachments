using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    class IncomingAttachment : IIncomingAttachment
    {
        IIncomingAttachments attachments;

        public IncomingAttachment(IIncomingAttachments attachments)
        {
            this.attachments = attachments;
        }

        public Task CopyTo(Stream target)
        {
            Guard.AgainstNull(target, nameof(target));
            return attachments.CopyTo(string.Empty, target);
        }

        public Task ProcessStream(Func<Stream, Task> action)
        {
            Guard.AgainstNull(action, nameof(action));
            return attachments.ProcessStream(string.Empty, action);
        }

        public Task<byte[]> GetBytes()
        {
            return attachments.GetBytes(string.Empty);
        }

        public Task<Stream> GetStream()
        {
            return attachments.GetStream(string.Empty);
        }
    }
}