
namespace NServiceBus.Attachments.Testing
{
    public partial class MockAttachmentService
    {
        public virtual IMessageAttachment BuildIncomingAttachment(IMessageHandlerContext context)
        {
            return new MockMessageAttachment(context);
        }

        public virtual IMessageAttachments BuildIncomingAttachments(IMessageHandlerContext context)
        {
            return new MockMessageAttachments(context);
        }

        public virtual IMessageAttachments BuildAttachmentsForMessage(IMessageHandlerContext context, string messageId)
        {
            return new MockMessageAttachments(context,messageId);
        }

        public virtual IMessageAttachment BuildAttachmentForMessage(IMessageHandlerContext context, string messageId)
        {
            return new MockMessageAttachment(context,messageId);
        }
    }
}