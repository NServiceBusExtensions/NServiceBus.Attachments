using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare.Testing
#endif
#if Sql
    .Sql.Testing
#endif
{
    /// <summary>
    /// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
    /// All members are stubbed out.
    /// </summary>
    /// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
    public partial class StubMessageAttachments :
        IMessageAttachments
    {
        string messageId;

        /// <summary>
        /// Instantiate a new instance of <see cref="StubMessageAttachments"/>.
        /// </summary>
        public StubMessageAttachments()
        {
            messageId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="StubMessageAttachments"/>.
        /// </summary>
        public StubMessageAttachments(string messageId)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            this.messageId = messageId;
        }

        /// <inheritdoc />
        public virtual Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
        {
            CopyCurrentMessageAttachmentToStream(name, target);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task CopyTo(Stream target, CancellationToken cancellation = default)
        {
            return CopyTo("default", target, cancellation);
        }

        /// <inheritdoc />
        public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return InnerProcessStream(name, action);
        }

        /// <inheritdoc />
        public virtual Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return ProcessStream("default", action, cancellation);
        }

        /// <inheritdoc />
        public virtual async Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            foreach (var pair in currentAttachments)
            {
                await using var attachmentStream = pair.Value.ToAttachmentStream();
                await action(attachmentStream);
            }
        }

        /// <inheritdoc />
        public virtual Task<AttachmentString> GetString(CancellationToken cancellation = default)
        {
            return GetString("default", cancellation);
        }

        /// <inheritdoc />
        public virtual Task<AttachmentString> GetString(string name, CancellationToken cancellation = default)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return Task.FromResult(attachment.ToAttachmentString());
        }

        /// <inheritdoc />
        public virtual Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
        {
            return GetBytes("default", cancellation);
        }

        /// <inheritdoc />
        public virtual Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return Task.FromResult(attachment.ToAttachmentBytes());
        }

        /// <inheritdoc />
        public virtual Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            using var writer = BuildWriter(target);
            writer.Write(attachment.Bytes);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
        {
            return CopyToForMessage(messageId, "default", target, cancellation);
        }

        /// <inheritdoc />
        public virtual async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            await using var attachmentStream = attachment.ToAttachmentStream();
            await action(attachmentStream);
        }

        /// <inheritdoc />
        public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return ProcessStreamForMessage("default", messageId, action, cancellation);
        }

        /// <inheritdoc />
        public virtual async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            foreach (var pair in GetAttachmentsForMessage(messageId))
            {
                var attachment = pair.Value;
                await using var attachmentStream = attachment.ToAttachmentStream();
                await action(attachmentStream);
            }
        }

        /// <inheritdoc />
        public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
        {
            return GetBytesForMessage(messageId, "default", cancellation);
        }

        /// <inheritdoc />
        public virtual Task<AttachmentString> GetStringForMessage(string messageId, CancellationToken cancellation = default)
        {
            return GetStringForMessage(messageId, "default", cancellation);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default)
        {
            var infos = currentAttachments.Select(_ => new AttachmentInfo(messageId, _.Key, _.Value.Expiry, _.Value.Metadata));
            return new AsyncEnumerable<AttachmentInfo>(infos);
        }

        /// <inheritdoc />
        public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return Task.FromResult(attachment.ToAttachmentBytes());
        }

        /// <inheritdoc />
        public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return Task.FromResult(attachment.ToAttachmentString());
        }

        void CopyCurrentMessageAttachmentToStream(string name, Stream target)
        {
            var bytes = GetCurrentMessageBytes(name);

            using var writer = BuildWriter(target);
            writer.Write(bytes);
        }

        byte[] GetCurrentMessageBytes(string name)
        {
            if (currentAttachments.TryGetValue(name, out var attachment))
            {
                return attachment.Bytes;
            }

            throw new Exception($"Cant find an attachment: {name}");
        }

        MockAttachment GetAttachmentForMessage(string messageId, string name)
        {
            var attachmentsForMessage = GetAttachmentsForMessage(messageId);
            if (attachmentsForMessage.TryGetValue(name, out var attachment))
            {
                return attachment;
            }

            throw new Exception($"Cant find an attachment: {name}");
        }

        Dictionary<string, MockAttachment> GetAttachmentsForMessage(string messageId)
        {
            if (attachments.TryGetValue(messageId, out var attachmentsForMessage))
            {
                return attachmentsForMessage;
            }

            throw new Exception($"Cant find an attachment: {messageId}");
        }

        MockAttachment GetCurrentMessageAttachment(string name)
        {
            if (currentAttachments.TryGetValue(name, out var attachment))
            {
                return attachment;
            }

            throw new Exception($"Cant find an attachment: {name}");
        }

        static BinaryWriter BuildWriter(Stream target)
        {
            return new BinaryWriter(target, Encoding.UTF8, leaveOpen: true);
        }

        Task InnerProcessStream(string name, Func<AttachmentStream, Task> action)
        {
            var attachment = GetCurrentMessageAttachment(name);
            using var attachmentStream = attachment.ToAttachmentStream();
            return action(attachmentStream);
        }
    }
}