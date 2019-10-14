using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql.Testing
{
    public partial class MockMessageAttachments
    {
        /// <inheritdoc />
        public virtual Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <inheritdoc />
        public virtual Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default )
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <inheritdoc />
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }

        /// <inheritdoc />
        public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            return Task.FromResult(AttachmentStream.Empty());
        }
    }
}