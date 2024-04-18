class MyHandler :
    IHandleMessages<SendMessage>,
    IHandleMessages<ReplyMessage>
{
    public async Task Handle(SendMessage message, HandlerContext context)
    {
        Console.WriteLine("Hello from MyHandler. SendMessage");
        var incomingAttachments = context.Attachments();
        var attachment = await incomingAttachments.GetString("foo", cancel: context.CancellationToken);
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var outgoingAttachments = sendOptions.Attachments();
        outgoingAttachments.AddString("bar", attachment);
        await context.Send(new ReplyMessage(), sendOptions);
    }

    public async Task Handle(ReplyMessage message, HandlerContext context)
    {
        var incomingAttachments = context.Attachments();
        var attachment = await incomingAttachments.GetString("bar", cancel: context.CancellationToken);
        Console.WriteLine($"Hello from MyHandler. ReplyMessage. {attachment.Value}");
    }
}