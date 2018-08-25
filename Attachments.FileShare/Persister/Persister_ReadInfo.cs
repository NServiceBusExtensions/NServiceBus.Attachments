using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments.
        /// </summary>
        public virtual IEnumerable<AttachmentInfo> ReadAllInfo()
        {
            return from messageDirectory in Directory.EnumerateDirectories(fileShare)
                let messageId = Path.GetFileName(messageDirectory)
                from attachmentInfo in ReadMessageInfo(messageDirectory, messageId)
                select attachmentInfo;
        }

        /// <summary>
        /// Reads the <see cref="AttachmentInfo"/> for all attachments of a specific message.
        /// </summary>
        public virtual IEnumerable<AttachmentInfo> ReadAllMessageInfo(string messageId)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            var messageDirectory = GetMessageDirectory(messageId);
            return ReadMessageInfo(messageDirectory, messageId);
        }

        IEnumerable<AttachmentInfo> ReadMessageInfo(string messageDirectory, string messageId)
        {
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                var expiryFile = Directory.EnumerateFiles(attachmentDirectory, "*.expiry").Single();
                var metadata = ReadMetadata(attachmentDirectory);
                yield return new AttachmentInfo(
                    messageId: messageId,
                    name: Path.GetFileName(attachmentDirectory),
                    expiry: ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile)),
                    metadata: metadata);
            }
        }
    }
}