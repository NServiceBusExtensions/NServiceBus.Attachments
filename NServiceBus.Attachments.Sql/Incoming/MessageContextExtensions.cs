using NServiceBus.Attachments;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IncomingAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            return context.Extensions.Get<IncomingAttachments>();
        }
        public static IncomingAttachment IncomingAttachment(this IMessageHandlerContext context)
        {
            var incomingAttachments = context.Extensions.Get<IncomingAttachments>();
            return new IncomingAttachment(incomingAttachments);
        }
    }
}