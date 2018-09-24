using System;
using System.Threading.Tasks;
using NServiceBus;

class MyHandler :
    IHandleMessages<SendMessage>,
    IHandleMessages<ReplyMessage>
{
    public async Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        Console.WriteLine("Hello from MyHandler. SendMessage");
        var incomingAttachments = context.Attachments();
        var attachment = await incomingAttachments.GetStream("foo");
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var outgoingAttachments = sendOptions.Attachments();
        outgoingAttachments.Add("bar", attachment);
        await context.Send(new ReplyMessage(), sendOptions);
    }

    public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var incomingAttachments = context.Attachments();
        var attachment = await incomingAttachments.GetBytes("bar");
        Console.WriteLine($"Hello from MyHandler. ReplyMessage. bytes: {attachment.Bytes.Length}");
    }
}