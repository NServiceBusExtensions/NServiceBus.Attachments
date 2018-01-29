namespace NServiceBus.Attachments.Testing
{
    public static class MockAttachmentHelper
    {
        public static void AddMockAttachmentService(this IMessageHandlerContext context, MockAttachmentService mockAttachmentService)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(mockAttachmentService, nameof(mockAttachmentService));
            context.Extensions.Set(mockAttachmentService);
        }
    }
}