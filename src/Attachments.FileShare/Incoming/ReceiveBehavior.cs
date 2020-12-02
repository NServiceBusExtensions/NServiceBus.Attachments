using System;
using System.Threading.Tasks;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    IPersister persister;

    public ReceiveBehavior(IPersister persister)
    {
        this.persister = persister;
    }

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        FileShareAttachmentState state = new(persister);
        context.Extensions.Set(state);
        return next();
    }
}