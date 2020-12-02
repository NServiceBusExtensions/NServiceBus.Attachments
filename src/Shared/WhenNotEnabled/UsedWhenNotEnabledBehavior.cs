using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

#if FileShare
using NServiceBus.Attachments.FileShare;
#elif Sql
using NServiceBus.Attachments.Sql;
#else
using NServiceBus.Attachments;
#endif

class UsedWhenNotEnabledBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    public const string Text = "Attachments used when not enabled. For example SendOptions.Attachments() was used but Attachments was not enabled via EndpointConfiguration.EnableAttachments().";

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        if (context.Extensions.TryGet<IOutgoingAttachments>(out _))
        {
            throw new(Text);
        }

        return next();
    }
}