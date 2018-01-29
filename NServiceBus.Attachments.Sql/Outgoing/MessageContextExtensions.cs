using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
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
    }
}