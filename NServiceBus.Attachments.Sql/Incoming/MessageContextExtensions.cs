using NServiceBus.Attachments;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IIncomingAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            return context.Extensions.Get<IncomingAttachments>();
        }

        public static IIncomingAttachment IncomingAttachment(this IMessageHandlerContext context)
        {
            var incomingAttachments = context.IncomingAttachments();
            return new IncomingAttachment(incomingAttachments);
        }
    }
}