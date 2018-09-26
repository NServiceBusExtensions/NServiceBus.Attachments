using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;

class ReplyHandler : IHandleMessages<ReplyMessage>
{
    public Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var incomingAttachment = context.Attachments();

        IntegrationTests.PerformNestedConnection();

        var buffer = incomingAttachment.GetBytes();
        Debug.WriteLine(buffer);
        using (var stream = incomingAttachment.GetStream())
        {
            Debug.WriteLine(stream);
        }

        IntegrationTests.HandlerEvent.Set();
        return Task.CompletedTask;
    }
}