using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
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
            var bytes = await FileHelpers.ReadBytes(cancellation, dataFile);
            var metadata = ReadMetadata(attachmentDirectory);
            return new AttachmentBytes(name, bytes, metadata);
        }

        /// <summary>
        /// Reads a string for an attachment.
        /// </summary>
        public virtual Task<AttachmentString> GetString(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var metadata = ReadMetadata(attachmentDirectory);
            var attachment = new AttachmentString(name, File.ReadAllText(dataFile), metadata);
            return Task.FromResult(attachment);
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
            return new AttachmentStream(name, read, read.Length, metadata);
        }
    }
}