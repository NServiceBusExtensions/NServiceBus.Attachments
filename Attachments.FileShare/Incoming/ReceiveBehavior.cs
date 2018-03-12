using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    Persister persister;

    public ReceiveBehavior(Persister persister)
    {
        this.persister = persister;
    }

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var state = new FileShareAttachmentState(persister);
        context.Extensions.Set(state);
        return next();
    }
}