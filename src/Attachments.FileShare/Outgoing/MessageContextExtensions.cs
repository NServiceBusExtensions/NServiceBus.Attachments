using NServiceBus.Attachments.FileShare;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    public static partial class FileShareAttachmentsMessageContextExtensions
    {
        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this PublishOptions options)
        {
            return GetAttachments(options);
        }

        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this SendOptions options)
        {
            return GetAttachments(options);
        }

        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this ReplyOptions options)
        {
            return GetAttachments(options);
        }

        static IOutgoingAttachments GetAttachments(this ExtendableOptions options)
        {
            Guard.AgainstNull(options, nameof(options));
            var contextBag = options.GetExtensions();
            // check the context for a IOutgoingAttachments in case a mocked instance is injected for testing
            if (contextBag.TryGet<IOutgoingAttachments>(out var attachments))
            {
                return attachments;
            }

            attachments = new OutgoingAttachments();
            contextBag.Set(attachments);
            return attachments;
        }
    }
}