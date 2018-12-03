using System.Threading.Tasks;
using NServiceBus;
using Xunit;

class SendHandler : IHandleMessages<SendMessage>
{
    public async Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var replyOptions = new SendOptions();
        replyOptions.RouteToThisEndpoint();
        var attachment = await context.Attachments().GetBytes("withMetadata");
        Assert.Equal("value", attachment.Metadata["key"]);
        Assert.NotNull(attachment);
        var outgoingAttachment = replyOptions.Attachments();
        outgoingAttachment.AddBytes(attachment);

        IntegrationTests.PerformNestedConnection();

        await context.Send(new ReplyMessage(), replyOptions);
    }
}