using NServiceBus;

class MyHandler :
    IHandleMessages<MyMessage>
{
    public async Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        Console.WriteLine("Hello from MyHandler.");
        var incomingAttachments = context.Attachments();
        Console.WriteLine(await incomingAttachments.GetString("foo"));
    }
}