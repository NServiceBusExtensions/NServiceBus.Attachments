using System;
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
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStreams(string messageId, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNull(action, nameof(action));
            var messageDirectory = GetMessageDirectory(messageId);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                cancellation.ThrowIfCancellationRequested();
                var dataFile = GetDataFile(attachmentDirectory);
                var attachmentName = Directory.GetParent(dataFile).Name;
                var read = FileHelpers.OpenRead(dataFile);
                var metadata = ReadMetadata(attachmentDirectory);
                using var fileStream = new AttachmentStream(read, read.Length, metadata);
                await action(attachmentName, fileStream);
            }
        }

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStream(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(action, nameof(action));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            ThrowIfDirectoryNotFound(attachmentDirectory, messageId);
            cancellation.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            var read = FileHelpers.OpenRead(dataFile);
            var metadata = ReadMetadata(attachmentDirectory);
            using var fileStream = new AttachmentStream(read, read.Length, metadata);
            await action(fileStream);
        }
    }
}