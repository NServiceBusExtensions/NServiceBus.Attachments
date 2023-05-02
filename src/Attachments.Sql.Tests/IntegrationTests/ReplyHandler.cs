class ReplyHandler :
    IHandleMessages<ReplyMessage>
{
    IntegrationTests integrationTests;

    public ReplyHandler(IntegrationTests integrationTests) =>
        this.integrationTests = integrationTests;

    public async Task Handle(ReplyMessage message, HandlerContext context)
    {
        var incomingAttachment = context.Attachments();

        integrationTests.PerformNestedConnection();

        var buffer = await incomingAttachment.GetBytes();
        Debug.WriteLine(buffer);
        await using var stream = await incomingAttachment.GetStream();
        Debug.WriteLine(stream);
        var attachmentInfos = await incomingAttachment.GetMetadata().ToAsyncList();
        Assert.Equal(1, attachmentInfos.Count);
        integrationTests.HandlerEvent.Set();
    }
}