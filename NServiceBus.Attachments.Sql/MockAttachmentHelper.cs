namespace NServiceBus.Attachments.Testing
{
    public static class MockAttachmentHelper
    {
        public static void InjectAttachmentsInstance(this IMessageHandlerContext context, IMessageAttachments messageAttachments)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageAttachments, nameof(messageAttachments));
            context.Extensions.Set(messageAttachments);
        }
    }
}