using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentInfo> ReadAllInfo(
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            foreach (var messageDirectory in Directory.EnumerateDirectories(fileShare))
            {
                var messageId = Path.GetFileName(messageDirectory);
                await foreach (var info in ReadMessageInfo(messageDirectory, messageId, cancellation))
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        yield break;
                    }
                    yield return info;
                }
            }
        }

        /// <inheritdoc />
        public virtual IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(
            string messageId,
            CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            var messageDirectory = GetMessageDirectory(messageId);
            return ReadMessageInfo(messageDirectory, messageId, cancellation);
        }

        async IAsyncEnumerable<AttachmentInfo> ReadMessageInfo(
            string messageDirectory,
            string messageId,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                if (cancellation.IsCancellationRequested)
                {
                    yield break;
                }
                var expiryFile = Directory.EnumerateFiles(attachmentDirectory, "*.expiry").Single();
                var metadata = await ReadMetadata(attachmentDirectory);
                yield return new AttachmentInfo(
                    messageId: messageId,
                    name: Path.GetFileName(attachmentDirectory),
                    expiry: ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile)),
                    metadata: metadata);
            }
        }
    }
}