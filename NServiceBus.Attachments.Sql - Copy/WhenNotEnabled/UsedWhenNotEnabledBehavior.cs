using System;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class UsedWhenNotEnabledBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    public const string Text = "Attachments used when not enabled. For example SendOptions.Attachments() was used but Attachments was not enabled via EndpointConfiguration.EnableAttachments().";

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        if (context.Extensions.TryGet<IOutgoingAttachments>(out var _))
        {
            throw new Exception(Text);
        }

        return next();
    }
}