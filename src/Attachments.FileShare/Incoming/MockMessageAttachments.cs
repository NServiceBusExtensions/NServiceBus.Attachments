namespace NServiceBus.Attachments.FileShare.Testing
{
    public partial class MockMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream()"/>
        /// </summary>
        public virtual AttachmentStream GetStream()
        {
            return AttachmentStream.Empty();
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string)"/>
        /// </summary>
        public virtual AttachmentStream GetStream(string name )
        {
            return AttachmentStream.Empty();
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string)"/>
        /// </summary>
        public virtual AttachmentStream GetStreamForMessage(string messageId)
        {
            return AttachmentStream.Empty();
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string)"/>
        /// </summary>
        public virtual AttachmentStream GetStreamForMessage(string messageId, string name)
        {
            return AttachmentStream.Empty();
        }
    }
}