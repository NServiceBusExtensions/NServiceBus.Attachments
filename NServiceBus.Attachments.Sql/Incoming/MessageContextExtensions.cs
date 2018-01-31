using System;
using NServiceBus.Attachments;

namespace NServiceBus
{
    public static partial class MessageContextExtensions
    {
        public static IMessageAttachments Attachments(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            var contextBag = context.Extensions;
            if (contextBag.TryGet<IMessageAttachments>(out var attachments))
            {
                return attachments;
            }

            if (!contextBag.TryGet<AttachmentReceiveState>(out var state))
            {
                throw new Exception("Attachments used when not enabled. For example IMessageHandlerContext.Attachments() was used but Attachments was not enabled via EndpointConfiguration.EnableAttachments().");
            }
            return new MessageAttachments(state.ConnectionFactory, context.MessageId, state.Persister);
        }
    }
}