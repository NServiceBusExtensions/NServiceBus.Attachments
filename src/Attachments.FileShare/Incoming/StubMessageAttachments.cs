namespace NServiceBus.Attachments.FileShare.Testing
{
    public partial class StubMessageAttachments
    {
        /// <inheritdoc />
        public virtual AttachmentStream GetStream()
        {
            return GetStream("default");
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStream(string name)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return attachment.ToAttachmentStream();
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStreamForMessage(string messageId)
        {
            return GetStreamForMessage(messageId, "default");
        }

        /// <inheritdoc />
        public virtual AttachmentStream GetStreamForMessage(string messageId, string name)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return attachment.ToAttachmentStream();
        }
    }
}