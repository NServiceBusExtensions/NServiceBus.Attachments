using NServiceBus.Attachments.Sql;

class SendHandler(IntegrationTests tests) :
    IHandleMessages<SendMessage>
{
    public async Task Handle(SendMessage message, HandlerContext context)
    {
        var replyOptions = new SendOptions();
        replyOptions.RouteToThisEndpoint();
        var incomingAttachments = context.Attachments();
        var attachment = await incomingAttachments.GetBytes("withMetadata");
        Assert.Equal("value", attachment.Metadata["key"]);
        Assert.NotNull(attachment);

        var directory = Path.Combine(AttributeReader.GetSolutionDirectory(), "temp");
        await incomingAttachments.CopyToDirectory(directory);

        var outgoingAttachment = replyOptions.Attachments();
        outgoingAttachment.AddBytes(attachment);

        var attachmentInfos = await incomingAttachments.GetMetadata().ToAsyncList();
        Assert.Equal(6, attachmentInfos.Count);
        tests.PerformNestedConnection();

        await context.Send(new ReplyMessage(), replyOptions);
    }
}