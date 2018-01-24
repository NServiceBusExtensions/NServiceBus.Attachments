using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static OutgoingAttachment OutgoingAttachment(this PublishOptions options)
        {
            return OutgoingAttachment(options);
        }

        public static OutgoingAttachment OutgoingAttachment(this SendOptions options)
        {
            return OutgoingAttachment(options);
        }

        public static OutgoingAttachment OutgoingAttachment(this ReplyOptions options)
        {
            return OutgoingAttachment(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this PublishOptions options)
        {
            return OutgoingAttachments(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this SendOptions options)
        {
            return OutgoingAttachments(options);
        }

        public static OutgoingAttachments OutgoingAttachments(this ReplyOptions options)
        {
            return OutgoingAttachments(options);
        }

        static OutgoingAttachments OutgoingAttachments(this ExtendableOptions options)
        {
            return options.GetExtensions().GetOrCreate<OutgoingAttachments>();
        }

        static OutgoingAttachment OutgoingAttachment(this ExtendableOptions options)
        {
            var outgoingAttachments = options.GetExtensions().GetOrCreate<OutgoingAttachments>();
            return new OutgoingAttachment(outgoingAttachments);
        }
    }
}