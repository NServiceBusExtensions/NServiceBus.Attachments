using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IOutgoingAttachment OutgoingAttachment(this PublishOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachment(this SendOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachment(this ReplyOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachments OutgoingAttachments(this PublishOptions options)
        {
            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachments(this SendOptions options)
        {
            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachments(this ReplyOptions options)
        {
            return GetOutgoingAttachments(options);
        }

        static IOutgoingAttachments GetOutgoingAttachments(this ExtendableOptions options)
        {
            var contextBag = options.GetExtensions();
            if (contextBag.TryGet<IOutgoingAttachments>(out var attachments))
            {
                return attachments;
            }

            attachments = new OutgoingAttachments();
            contextBag.Set(attachments);
            return attachments;
        }

        static IOutgoingAttachment GetOutgoingAttachment(this ExtendableOptions options)
        {
            if (options.GetExtensions().TryGet<IOutgoingAttachment>(out var attachment))
            {
                return attachment;
            }
            return new OutgoingAttachment(GetOutgoingAttachments(options));
        }
    }
}