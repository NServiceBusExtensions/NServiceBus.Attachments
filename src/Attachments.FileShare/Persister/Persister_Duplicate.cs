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
        /// Copies an attachment to a different message.
        /// </summary>
        public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
            Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
            var targetDirectory = GetAttachmentDirectory(targetMessageId, name);
            var sourceDataFile = GetDataFile(sourceDirectory);
            ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
            FileHelpers.Copy(sourceDirectory, targetDirectory);
            return Task.CompletedTask;
        }
    }
}