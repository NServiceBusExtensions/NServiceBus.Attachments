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
        /// <summary>
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(stream, nameof(stream));
            return Save(messageId, name, expiry, metadata, fileStream => stream.CopyToAsync(fileStream, 4096, cancellation));
        }

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            return Save(messageId, name, expiry, metadata, fileStream => fileStream.WriteAsync(bytes, 0, bytes.Length, cancellation));
        }

        /// <summary>
        /// Saves <paramref name="value"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveString(string messageId, string? name, DateTime expiry, string value, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));
            return Save(messageId, name, expiry, metadata, async fileStream =>
            {
                await using var writer = fileStream.BuildLeaveOpenWriter();
                await writer.WriteLineAsync(value);
            });
        }

        async Task Save(string messageId, string? name, DateTime expiry, IReadOnlyDictionary<string, string>? metadata, Func<FileStream, Task> action)
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
            await using (File.Create(expiryFile))
            {
            }
            WriteMetadata(attachmentDirectory, metadata);

            await using var fileStream = FileHelpers.OpenWrite(dataFile);
            await action(fileStream);
        }
    }
}