using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(string, Stream, CancellationToken)"/>
        /// </summary>
        public virtual Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
        {
            CopyCurrentMessageAttachmentToStream(name, target);
            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyTo(Stream,CancellationToken)"/>
        /// </summary>
        public virtual Task CopyTo(Stream target, CancellationToken cancellation = default)
        {
            return CopyTo("default", target, cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(string, Func{AttachmentStream, Task}, CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return InnerProcessStream(name, action);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream, Task}, CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return ProcessStream("default", action, cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreams"/>
        /// </summary>
        public virtual async Task ProcessStreams(Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            foreach (var pair in currentAttachments)
            {
                using (var attachmentStream = pair.Value.ToAttachmentStream())
                {
                    await action(pair.Key, attachmentStream);
                }
            }
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetString(CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentString> GetString(CancellationToken cancellation = default)
        {
            return GetString("default", cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetString(string ,CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentString> GetString(string name, CancellationToken cancellation = default)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return Task.FromResult(attachment.ToAttachmentString());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes(CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
        {
            return GetBytes("default", cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytes(string, CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return Task.FromResult(attachment.ToAttachmentBytes());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string, string, Stream, CancellationToken)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            using (var writer = BuildWriter(target))
            {
                writer.Write(attachment.Bytes);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.CopyToForMessage(string, Stream, CancellationToken)"/>
        /// </summary>
        public virtual Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
        {
            return CopyToForMessage(messageId, "default", target, cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string, string, Func{AttachmentStream, Task}, CancellationToken)"/>
        /// </summary>
        public virtual async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            using (var attachmentStream = attachment.ToAttachmentStream())
            {
                await action(attachmentStream);
            }
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamForMessage(string, Func{AttachmentStream, Task}, CancellationToken)"/>
        /// </summary>
        public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            return ProcessStreamForMessage("default", messageId, action, cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string, Func{string, AttachmentStream, Task}, CancellationToken)"/>
        /// </summary>
        public virtual async Task ProcessStreamsForMessage(string messageId, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
        {
            foreach (var pair in GetAttachmentsForMessage(messageId))
            {
                var attachment = pair.Value;
                using (var attachmentStream = attachment.ToAttachmentStream())
                {
                    await action(pair.Key, attachmentStream);
                }
            }
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string, CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
        {
            return GetBytesForMessage(messageId, "default", cancellation);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStringForMessage(string, CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentString> GetStringForMessage(string messageId, CancellationToken cancellation = default)
        {
            return GetStringForMessage(messageId, "default", cancellation);
        }

        /// <summary>
        /// Read all attachment metadata for the current message.
        /// </summary>
        public Task<IReadOnlyCollection<AttachmentInfo>> GetMetadata(CancellationToken cancellation = default)
        {
            var list = new List<AttachmentInfo>();
            foreach (var attachment in currentAttachments)
            {
                list.Add(new AttachmentInfo(messageId, attachment.Key, attachment.Value.Expiry, attachment.Value.Metadata));
            }

            return Task.FromResult<IReadOnlyCollection<AttachmentInfo>>(list);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetBytesForMessage(string, string, CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return Task.FromResult(attachment.ToAttachmentBytes());
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStringForMessage(string, string, CancellationToken)"/>
        /// </summary>
        public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return Task.FromResult(attachment.ToAttachmentString());
        }

        void CopyCurrentMessageAttachmentToStream(string name, Stream target)
        {
            var bytes = GetCurrentMessageBytes(name);

            using (var writer = BuildWriter(target))
            {
                writer.Write(bytes);
            }
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
            using (var attachmentStream = attachment.ToAttachmentStream())
            {
                return action(attachmentStream);
            }
        }
    }
}