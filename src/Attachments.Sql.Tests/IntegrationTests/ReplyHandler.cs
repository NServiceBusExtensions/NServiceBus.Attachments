class ReplyHandler :
    IHandleMessages<ReplyMessage>
{
    IntegrationTests integrationTests;

    public ReplyHandler(IntegrationTests integrationTests) =>
        this.integrationTests = integrationTests;

    public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var incomingAttachment = context.Attachments();

        integrationTests.PerformNestedConnection();

        var buffer = await incomingAttachment.GetBytes();
        Debug.WriteLine(buffer);
        using var stream = await incomingAttachment.GetStream();
        Debug.WriteLine(stream);
        integrationTests.HandlerEvent.Set();
    }
}