using NServiceBus.Attachments.FileShare;
using NServiceBus.Pipeline;

class ReceiveBehavior :
    Behavior<IInvokeHandlerContext>
{
    IPersister persister;

    public ReceiveBehavior(IPersister persister) =>
        this.persister = persister;

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var state = new FileShareAttachmentState(persister);
        context.Extensions.Set(state);
        return next();
    }
}