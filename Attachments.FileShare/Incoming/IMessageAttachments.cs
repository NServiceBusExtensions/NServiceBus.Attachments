using System.IO;

namespace NServiceBus.Attachments.FileShare
{
    public partial interface IMessageAttachments
    {
        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Stream GetStream();

        /// <summary>
        /// Get a <see cref="Stream"/>, for the current message, the attachment of <paramref name="name"/>.
        /// </summary>
        Stream GetStream(string name);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment with the default name of <see cref="string.Empty"/>.
        /// </summary>
        Stream GetStreamForMessage(string messageId);

        /// <summary>
        /// Get a <see cref="Stream"/>, for the message with <paramref name="messageId"/>, the attachment of <paramref name="name"/>.
        /// </summary>
        Stream GetStreamForMessage(string messageId, string name);
    }
}