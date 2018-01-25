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

        static OutgoingAttachments GetOutgoingAttachments(this ExtendableOptions options)
        {
            return options.GetExtensions().GetOrCreate<OutgoingAttachments>();
        }

        static OutgoingAttachment GetOutgoingAttachment(this ExtendableOptions options)
        {
            return new OutgoingAttachment(GetOutgoingAttachments(options));
        }
    }
}