using System.Threading.Tasks;
using NServiceBus;
using Xunit;

class SendHandler :
    IHandleMessages<SendMessage>
{
    IntegrationTests integrationTests;

    public SendHandler(IntegrationTests integrationTests)
    {
        this.integrationTests = integrationTests;
    }

    public async Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var replyOptions = new SendOptions();
        replyOptions.RouteToThisEndpoint();
        var attachment = await context.Attachments().GetBytes("withMetadata");
        Assert.Equal("value", attachment.Metadata["key"]);
        Assert.NotNull(attachment);
        var outgoingAttachment = replyOptions.Attachments();
        outgoingAttachment.AddBytes(attachment);

        integrationTests.PerformNestedConnection();

        await context.Send(new ReplyMessage(), replyOptions);
    }
}