using System;
using System.Threading;
using NServiceBus.Attachments;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    public static partial class MessageContextExtensions
    {
        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this PublishOptions options, CancellationToken cancellation = default)
        {
            return GetAttachments(options, cancellation);
        }

        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this SendOptions options, CancellationToken cancellation = default)
        {
            return GetAttachments(options, cancellation);
        }

        /// <summary>
        /// Provides an instance of <see cref="IOutgoingAttachments"/> for writing attachments.
        /// </summary>
        public static IOutgoingAttachments Attachments(this ReplyOptions options, CancellationToken cancellation = default)
        {
            return GetAttachments(options, cancellation);
        }

        static IOutgoingAttachments GetAttachments(this ExtendableOptions options, CancellationToken cancellation)
        {
            var contextBag = options.GetExtensions();
            if (contextBag.TryGet<IOutgoingAttachments>(out var attachments))
            {
                if (attachments is OutgoingAttachments outgoingAttachments)
                {
                    if (outgoingAttachments.Cancellation != cancellation)
                    {
                        throw new Exception("Changing the CancellationToken for outgoing attachments is not supported.");
                    }
                }
                return attachments;
            }

            attachments = new OutgoingAttachments(cancellation);
            contextBag.Set(attachments);
            return attachments;
        }
    }
}