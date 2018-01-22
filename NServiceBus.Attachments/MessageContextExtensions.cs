using NServiceBus.Extensibility;

namespace NServiceBus.Attachments
{
    public static class MessageContextExtensions
    {
        public static IncomingAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            return context.Extensions.Get<IncomingAttachments>();
        }

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