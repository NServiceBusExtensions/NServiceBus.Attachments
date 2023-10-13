namespace NServiceBus.Attachments
#if FileShare
.FileShare.Testing
#elif Sql
.Sql.Testing
#endif
;

/// <summary>
/// Attachment testing helpers
/// </summary>
public static class MockAttachmentHelper
{
    /// <summary>
    /// Inject an instance of <see cref="IMessageAttachments"/> into a <see cref="IMessageHandlerContext"/> to allow it to be mocked and asserted against.
    /// </summary>
    public static void InjectAttachmentsInstance(this HandlerContext context, IMessageAttachments messageAttachments) =>
        context.Extensions.Set(messageAttachments);
}