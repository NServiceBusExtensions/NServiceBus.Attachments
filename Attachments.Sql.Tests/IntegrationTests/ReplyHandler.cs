using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;

class ReplyHandler : IHandleMessages<ReplyMessage>
{
    public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var incomingAttachment = context.Attachments();

        IntegrationTests.PerformNestedConnection();

        var buffer = await incomingAttachment.GetBytes();
        Debug.WriteLine(buffer);
        using (var stream = await incomingAttachment.GetStream())
        {
            Debug.WriteLine(stream);
        }

        IntegrationTests.HandlerEvent.Set();
    }
}