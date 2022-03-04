using NServiceBus.Attachments.FileShare;

namespace NServiceBus;

/// <summary>
/// Contextual extensions to manipulate attachments.
/// </summary>
public static partial class FileShareAttachmentsMessageContextExtensions
{
    /// <summary>
    /// Provides an instance of <see cref="IMessageAttachments" /> for reading attachments.
    /// </summary>
    public static IMessageAttachments Attachments(this IMessageHandlerContext context)
    {
        var contextBag = context.Extensions;
        // check the context for a IMessageAttachments in case a mocked instance is injected for testing
        if (contextBag.TryGet<IMessageAttachments>(out var attachments))
        {
            return attachments;
        }

        if (contextBag.TryGet<FileShareAttachmentState>(out var state))
        {
            return new MessageAttachments(context.MessageId, state.Persister);
        }

        throw new($"Attachments used when not enabled. For example IMessageHandlerContext.{nameof(Attachments)}() was used but Attachments was not enabled via EndpointConfiguration.{nameof(FileShareAttachmentsExtensions.EnableAttachments)}().");
    }
}