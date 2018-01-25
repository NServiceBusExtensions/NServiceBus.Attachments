using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachment
    {
        IncomingAttachments attachments;

        public IncomingAttachment(IncomingAttachments attachments)
        {
            this.attachments = attachments;
        }

        public Task CopyTo(Stream target)
        {
            Guard.AgainstNull(target,nameof(target));
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
    }
}