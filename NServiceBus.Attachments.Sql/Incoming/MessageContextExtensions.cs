using NServiceBus.Attachments;

namespace NServiceBus
{
    public static partial class MessageContextExtensions
    {
        public static IncomingAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            return context.Extensions.Get<IncomingAttachments>();
        }
    }
}