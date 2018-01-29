using NServiceBus.Attachments;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IMessageAttachments Attachments(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.Extensions.TryGet<IMessageAttachments>(out var attachments))
            {
                return attachments;
            }

            var state = context.Extensions.Get<AttachmentReceiveState>();
            return new MessageAttachments(state.ConnectionFactory, context.MessageId, state.Persister);
        }
    }
}