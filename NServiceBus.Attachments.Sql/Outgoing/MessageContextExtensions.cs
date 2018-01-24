using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static OutgoingAttachment OutgoingAttachment(this PublishOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static OutgoingAttachment OutgoingAttachment(this SendOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static OutgoingAttachment OutgoingAttachment(this ReplyOptions options)
        {
            return GetOutgoingAttachment(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this PublishOptions options)
        {
            return GetOutgoingAttachments(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this SendOptions options)
        {
            return GetOutgoingAttachments(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this ReplyOptions options)
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