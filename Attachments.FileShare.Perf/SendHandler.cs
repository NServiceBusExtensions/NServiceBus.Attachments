using System.Threading.Tasks;
using NServiceBus;

class SendHandler : IHandleMessages<SendMessage>
{
    public Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var replyOptions = new ReplyOptions();
        var outgoingAttachment = replyOptions.Attachments();
        var incomingAttachment = context.Attachments();
        outgoingAttachment.Add(incomingAttachment.GetStream);
        return context.Reply(new ReplyMessage(), replyOptions);
    }
}