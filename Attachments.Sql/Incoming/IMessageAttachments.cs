using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// Provides access to read attachments.
    /// </summary>
    public partial interface IMessageAttachments
    {
        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task<Stream> GetStream(CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment of <paramref name="name"/>.
        /// </summary>
        Task<Stream> GetStream(string name, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Task<Stream> GetStreamForMessage(string messageId, CancellationToken cancellation = default);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
        /// </summary>
        Task<Stream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default);
    }
}