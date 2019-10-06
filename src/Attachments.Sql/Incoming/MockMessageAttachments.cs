using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql.Testing
{
    public partial class MockMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default )
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }
    }
}