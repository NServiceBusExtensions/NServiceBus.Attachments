﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
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
            foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
            {
                cancellation.ThrowIfCancellationRequested();
                var attachmentName = Directory.GetParent(dataFile).Name;
                using (var fileStream = OpenAttachmentStream(dataFile))
                {
                    await action(attachmentName, fileStream).ConfigureAwait(false);
                }
            }
        }

        static AttachmentStream OpenAttachmentStream(string dataFile)
        {
            var read = FileHelpers.OpenRead(dataFile);
            return new AttachmentStream(read, read.Length);
        }

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        public virtual async Task ProcessStream(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(action, nameof(action));
            var messageDirectory = GetAttachmentDirectory(messageId, name);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
            {
                cancellation.ThrowIfCancellationRequested();
                using (var fileStream = OpenAttachmentStream(dataFile))
                {
                    await action(fileStream).ConfigureAwait(false);
                }
            }
        }
    }
}