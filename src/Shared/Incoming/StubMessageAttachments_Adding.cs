using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare.Testing
#endif
#if Sql
    .Sql.Testing
#endif
{
    public partial class StubMessageAttachments
    {
        Dictionary<string, MockAttachment> currentAttachments = new Dictionary<string, MockAttachment>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, Dictionary<string, MockAttachment>> attachments = new Dictionary<string, Dictionary<string, MockAttachment>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachment(string payload, IDictionary<string, string> metadata = null)
        {
            AddAttachment("default", payload, metadata);
        }

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachment(string name, string payload, IDictionary<string, string> metadata = null)
        {
            Guard.AgainstNull(payload, nameof(payload));
            AddAttachment(name, Encoding.UTF8.GetBytes(payload), metadata);
        }

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachment(byte[] bytes, IDictionary<string, string> metadata = null)
        {
            AddAttachment("default", bytes, metadata);
        }

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachment(string name, byte[] bytes, IDictionary<string, string> metadata=null)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            currentAttachments.Add(name,
                new MockAttachment
                {
                    Bytes = bytes,
                    Metadata = BuildMetadata(metadata),
                    Expiry = DateTime.UtcNow.AddDays(10)
                });
        }

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachmentForMessage(string messageId, byte[] bytes, IDictionary<string, string> metadata = null)
        {
            AddAttachmentForMessage(messageId, "default", bytes, metadata);
        }

        /// <summary>
        /// Adds a attachment that can then be used in a test.
        /// </summary>
        public void AddAttachmentForMessage(string messageId, string name, byte[] bytes, IDictionary<string, string> metadata = null)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            if (!attachments.TryGetValue(messageId, out var attachmentsForMessage))
            {
                attachments[messageId] = attachmentsForMessage = new Dictionary<string, MockAttachment>();
            }

            attachmentsForMessage.Add(name,
                new MockAttachment
                {
                    Bytes = bytes,
                    Metadata = BuildMetadata(metadata),
                    Expiry = DateTime.UtcNow.AddDays(10)
                });
        }

        static IReadOnlyDictionary<string, string> BuildMetadata(IDictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                return new Dictionary<string, string>();
            }

            return metadata.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}