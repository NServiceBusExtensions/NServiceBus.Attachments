namespace NServiceBus.Attachments.FileShare.Testing
{
    public partial class MockMessageAttachments
    {
        /// <inheritdoc />
        public virtual AttachmentStream GetStream()
        {
            return AttachmentStream.Empty();
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStream(string name )
        {
            return AttachmentStream.Empty();
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStreamForMessage(string messageId)
        {
            return AttachmentStream.Empty();
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStreamForMessage(string messageId, string name)
        {
            return AttachmentStream.Empty();
        }
    }
}