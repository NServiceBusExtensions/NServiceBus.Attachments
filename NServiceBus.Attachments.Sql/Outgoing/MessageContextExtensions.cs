using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    public static partial class MessageContextExtensions
    {
        public static OutgoingAttachments OutgoingAttachments(this PublishOptions options)
        {
            return OutgoingAttachments((ExtendableOptions)options);
        }

        public static OutgoingAttachments OutgoingAttachments(this SendOptions options)
        {
            return OutgoingAttachments((ExtendableOptions)options);
        }

        public static OutgoingAttachments OutgoingAttachments(this ReplyOptions options)
        {
            return OutgoingAttachments((ExtendableOptions)options);
        }

        static OutgoingAttachments OutgoingAttachments(this ExtendableOptions options)
        {
            var outgoingAttachments = new OutgoingAttachments();
            options.GetExtensions().Set(outgoingAttachments);
            return outgoingAttachments;
        }
    }
}