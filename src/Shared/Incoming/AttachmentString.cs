#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
    public class AttachmentString :
        IAttachment
    {
        /// <summary>
        /// An empty <see cref="AttachmentString"/> that contains a "default" name and <see cref="string.Empty"/> contents.
        /// </summary>
        public static AttachmentString Empty = new("default", string.Empty);

        /// <summary>
        /// The attachment value.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Metadata { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="AttachmentStream"/>.
        /// </summary>
        /// <param name="name">The name of the attachment.</param>
        /// <param name="value">The <see cref="string"/>s to wrap.</param>
        /// <param name="metadata">The attachment metadata.</param>
        public AttachmentString(string name, string value, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            metadata ??= MetadataSerializer.EmptyMetadata;

            Name = name;
            Value = value;
            Metadata = metadata;
        }


        public static implicit operator string(AttachmentString d)
        {
            return d.Value;
        }
    }
}