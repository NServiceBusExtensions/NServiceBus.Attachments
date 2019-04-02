namespace NServiceBus.Attachments.FileShare.Testing
{
    public partial class StubMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream()"/>
        /// </summary>
        public virtual AttachmentStream GetStream()
        {
            return GetStream("default");
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string)"/>
        /// </summary>
        public virtual AttachmentStream GetStream(string name)
        {
            var attachment = GetCurrentMessageAttachment(name);
            return attachment.ToAttachmentStream();
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string)"/>
        /// </summary>
        public virtual AttachmentStream GetStreamForMessage(string messageId)
        {
            return GetStreamForMessage(messageId, "default");
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string)"/>
        /// </summary>
        public virtual AttachmentStream GetStreamForMessage(string messageId, string name)
        {
            var attachment = GetAttachmentForMessage(messageId, name);
            return attachment.ToAttachmentStream();
        }
    }
}