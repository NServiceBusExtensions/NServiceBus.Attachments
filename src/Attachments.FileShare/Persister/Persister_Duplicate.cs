﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public virtual Task<IReadOnlyCollection<string>> Duplicate(string sourceMessageId, string targetMessageId, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
            Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
            var sourceDirectory = GetMessageDirectory(sourceMessageId);
            var targetDirectory = GetMessageDirectory(targetMessageId);
            FileHelpers.Copy(sourceDirectory, targetDirectory);
            List<string> names = new(Directory.EnumerateDirectories(targetDirectory).Select(Path.GetFileName));
            return Task.FromResult<IReadOnlyCollection<string>>(names);
        }

        /// <inheritdoc />
        public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, string targetName, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
            Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
            Guard.AgainstNullOrEmpty(targetName, nameof(targetName));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
            var targetDirectory = GetAttachmentDirectory(targetMessageId, targetName);
            var sourceDataFile = GetDataFile(sourceDirectory);
            ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
            FileHelpers.Copy(sourceDirectory, targetDirectory);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
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