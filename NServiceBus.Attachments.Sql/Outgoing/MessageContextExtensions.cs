using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IOutgoingAttachments Attachments(this PublishOptions options)
        {
            return GetAttachments(options);
        }

        public static IOutgoingAttachments Attachments(this SendOptions options)
        {
            return GetAttachments(options);
        }

        public static IOutgoingAttachments Attachments(this ReplyOptions options)
        {
            return GetAttachments(options);
        }

        static IOutgoingAttachments GetAttachments(this ExtendableOptions options)
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