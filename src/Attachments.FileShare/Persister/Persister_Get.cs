using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var bytes = await FileHelpers.ReadBytes(cancellation, dataFile);
            var metadata = await ReadMetadata(attachmentDirectory, cancellation);
            return new AttachmentBytes(name, bytes, metadata);
        }

        /// <inheritdoc />
        public virtual async Task<AttachmentString> GetString(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var metadata = await ReadMetadata(attachmentDirectory, cancellation);
            var allText = File.ReadAllText(dataFile);
            return new AttachmentString(name, allText, metadata);
        }

        /// <inheritdoc />
        public virtual Task<AttachmentStream> GetStream(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            return OpenAttachmentStream(messageId, name, cancellation);
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentBytes> GetBytes(string messageId, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            var messageDirectory = GetMessageDirectory(messageId);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                cancellation.ThrowIfCancellationRequested();
                var dataFile = GetDataFile(attachmentDirectory);
                var attachmentName = Directory.GetParent(dataFile).Name;
                var bytes = await FileHelpers.ReadBytes(cancellation, dataFile);
                var metadata = await ReadMetadata(attachmentDirectory, cancellation);
                yield return new AttachmentBytes(attachmentName, bytes, metadata);
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentString> GetStrings(string messageId, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            var messageDirectory = GetMessageDirectory(messageId);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                cancellation.ThrowIfCancellationRequested();
                var dataFile = GetDataFile(attachmentDirectory);
                var attachmentName = Directory.GetParent(dataFile).Name;
                var metadata = await ReadMetadata(attachmentDirectory, cancellation);
                var allText = File.ReadAllText(dataFile);
                yield return new AttachmentString(attachmentName, allText, metadata);
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentStream> GetStreams(string messageId, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            var messageDirectory = GetMessageDirectory(messageId);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                cancellation.ThrowIfCancellationRequested();
                var dataFile = GetDataFile(attachmentDirectory);
                var attachmentName = Directory.GetParent(dataFile).Name;
                using var read = FileHelpers.OpenRead(dataFile);
                var metadata = await ReadMetadata(attachmentDirectory, cancellation);
                yield return new AttachmentStream(attachmentName, read, read.Length, metadata);
            }
        }

        async Task<AttachmentStream> OpenAttachmentStream(
            string messageId,
            string name,
            CancellationToken cancellation = default)
        {
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var metadata = await ReadMetadata(attachmentDirectory, cancellation);
            var read = FileHelpers.OpenRead(dataFile);
            return new AttachmentStream(name, read, read.Length, metadata);
        }
    }
}