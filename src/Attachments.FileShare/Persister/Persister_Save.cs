using System;
using System.Collections.Generic;
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
        /// <inheritdoc />
        public virtual Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(stream, nameof(stream));
            return Save(messageId, name, expiry, metadata, fileStream => stream.CopyToAsync(fileStream, 4096, cancellation), cancellation);
        }

        /// <inheritdoc />
        public virtual Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            return Save(messageId, name, expiry, metadata, fileStream => fileStream.WriteAsync(bytes, 0, bytes.Length, cancellation), cancellation);
        }

        /// <inheritdoc />
        public virtual Task SaveString(string messageId, string? name, DateTime expiry, string value, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));
            return Save(messageId, name, expiry, metadata,
                async fileStream =>
                {
                    using var writer = fileStream.BuildLeaveOpenWriter();
                    await writer.WriteAsync(value);
                },
                cancellation);
        }

        async Task Save(
            string messageId,
            string? name,
            DateTime expiry,
            IReadOnlyDictionary<string, string>? metadata,
            Func<FileStream, Task> action,
            CancellationToken cancellation = default)
        {
            if (name == null)
            {
                name = "default";
            }

            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            ThrowIfDirectoryExists(attachmentDirectory, messageId, name);

            Directory.CreateDirectory(attachmentDirectory);
            var dataFile = Path.Combine(attachmentDirectory, "data");
            expiry = expiry.ToUniversalTime();
            var expiryFile = Path.Combine(attachmentDirectory, $"{expiry:yyyy-MM-ddTHHmm}.expiry");
            using (File.Create(expiryFile))
            {
            }
            await WriteMetadata(attachmentDirectory, metadata, cancellation);

            using var fileStream = FileHelpers.OpenWrite(dataFile);
            await action(fileStream);
        }
    }
}