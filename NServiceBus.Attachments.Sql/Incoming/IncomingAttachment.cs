using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class IncomingAttachment
    {
        IncomingAttachments incomingAttachments;

        public IncomingAttachment(IncomingAttachments incomingAttachments)
        {
            this.incomingAttachments = incomingAttachments;
        }

        public Task CopyTo(Stream target)
        {
            return incomingAttachments.CopyTo(string.Empty, target);
        }

        public Task ProcessStream(Func<Stream, Task> action)
        {
            return incomingAttachments.ProcessStream(string.Empty, action);
        }

        public Task<byte[]> GetBytes()
        {
            return incomingAttachments.GetBytes(string.Empty);
        }
    }
}