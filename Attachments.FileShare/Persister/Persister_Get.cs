using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    public partial class Persister
    {
        /// <summary>
        /// Reads a byte array for an attachment.
        /// </summary>
        public virtual Task<byte[]> GetBytes(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var dataFile = GetDataFile(messageId, name);
            ThrowIfFileNotFound(dataFile, messageId, name);
            return FileHelpers.ReadBytes(cancellation, dataFile);
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
            var dataFile = GetDataFile(messageId, name);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var read = FileHelpers.OpenRead(dataFile);
            return new AttachmentStream(read, read.Length);
        }
    }
}