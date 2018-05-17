using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    public partial class Persister
    {
        /// <summary>
        /// Reads a byte array for an attachment.
        /// </summary>
        public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var bytes = await FileHelpers.ReadBytes(cancellation, dataFile).ConfigureAwait(false);
            var metadata = ReadMetadata(attachmentDirectory);
            return new AttachmentBytes(bytes, metadata);
        }

        /// <summary>
        /// Returns an open stream pointing to an attachment.
        /// </summary>
        public virtual AttachmentStream GetStream(string messageId, string name)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            return OpenAttachmentStream(messageId, name);
        }

        AttachmentStream OpenAttachmentStream(string messageId, string name)
        {
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var metadata = ReadMetadata(attachmentDirectory);
            var read = FileHelpers.OpenRead(dataFile);
            return new AttachmentStream(read, read.Length, metadata);
        }
    }
}