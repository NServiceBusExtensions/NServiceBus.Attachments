#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#endif
#if Sql
    .Sql
#endif
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Wraps a <see cref="string"/> to provide extra information when reading.
    /// </summary>
    public class AttachmentString
    {
        /// <summary>
        /// An empty <see cref="AttachmentString"/> that contains a <see cref="string.Empty"/>.
        /// </summary>
        public static AttachmentString Empty = new AttachmentString(string.Empty);

        /// <summary>
        /// The attachment bytes.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The attachment metadata.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Metadata;

        /// <summary>
        /// Initialises a new instance of <see cref="AttachmentStream"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/>s to wrap.</param>
        /// <param name="metadata">The attachment metadata.</param>
        public AttachmentString(string value, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Guard.AgainstNull(value, nameof(value));
            if (metadata == null)
            {
                metadata = MetadataSerializer.EmptyMetadata;
            }
            Value = value;
            Metadata = metadata;
        }

        public static implicit operator string(AttachmentString d)
        {
            return d.Value;
        }
    }
}