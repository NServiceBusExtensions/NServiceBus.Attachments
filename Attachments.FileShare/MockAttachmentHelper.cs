namespace NServiceBus.Attachments.FileShare.Testing
{
    /// <summary>
    /// Attachment testing helpers
    /// </summary>
    public static class MockAttachmentHelper
    {
        /// <summary>
        /// Inject an instance of <see cref="IMessageAttachments"/> into a <see cref="IMessageHandlerContext"/> to allow it to be mocked and asserted against.
        /// </summary>
        public static void InjectAttachmentsInstance(this IMessageHandlerContext context, IMessageAttachments messageAttachments)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageAttachments, nameof(messageAttachments));
            context.Extensions.Set(messageAttachments);
        }
    }
}