namespace NServiceBus.Attachments.Testing
{
    public class MockMessageAttachmentService
    {
        public virtual IMessageAttachment BuildAttachment(IMessageHandlerContext context)
        {
            return new MockMessageAttachment(context);
        }

        public virtual IMessageAttachments BuildAttachments(IMessageHandlerContext context)
        {
            return new MockMessageAttachments(context);
        }

        public virtual IMessageAttachments BuildAttachmentsForMessage(IMessageHandlerContext context, string messageId)
        {
            return new MockMessageAttachments(context, messageId);
        }

        public virtual IMessageAttachment BuildAttachmentForMessage(IMessageHandlerContext context, string messageId)
        {
            return new MockMessageAttachment(context, messageId);
        }
    }
}