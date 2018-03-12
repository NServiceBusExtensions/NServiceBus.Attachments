using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using NServiceBus.Attachments;

#if FileShare
using NServiceBus.Attachments.FileShare;
#endif
#if Sql
using NServiceBus.Attachments.Sql;
#endif

class UsedWhenNotEnabledBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    public const string Text = "Attachments used when not enabled. For example SendOptions.Attachments() was used but Attachments was not enabled via EndpointConfiguration.EnableAttachments().";

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        if (context.Extensions.TryGet<IOutgoingAttachments>(out _))
        {
            throw new Exception(Text);
        }

        return next();
    }
}