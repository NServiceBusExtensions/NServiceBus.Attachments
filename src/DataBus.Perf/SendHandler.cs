using System.Threading.Tasks;
using NServiceBus;

class SendHandler : IHandleMessages<SendMessage>
{
    public Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var replyOptions = new ReplyOptions();
        var replyMessage = new ReplyMessage
        {
            Blob = new DataBusProperty<byte[]>(message.Blob.Value)
        };
        return context.Reply(replyMessage, replyOptions);
    }
}