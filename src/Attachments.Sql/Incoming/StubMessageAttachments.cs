using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql.Testing
{
    public partial class StubMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
        {
            return GetStream("default", cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
        {
            var attachment = GetCurrentMessageAttachment(name);
            var attachmentStream = attachment.ToAttachmentStream();
            return Task.FromResult(attachmentStream);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
        {
            return GetStreamForMessage(messageId, "default", cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return Task.FromResult(attachment.ToAttachmentStream());
        }
    }
}