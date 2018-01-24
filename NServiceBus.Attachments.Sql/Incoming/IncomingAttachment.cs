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

        public Task<byte[]> GetBytes()
        {
            return incomingAttachments.GetBytes(string.Empty);
        }
    }
}