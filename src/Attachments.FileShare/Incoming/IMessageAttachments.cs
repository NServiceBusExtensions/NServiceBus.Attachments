using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    public partial interface IMessageAttachments
    {
        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task<AttachmentStream> GetStream(CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment of <paramref name="name"/>.
        /// </summary>
        Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
        /// </summary>
        Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default);
    }
}