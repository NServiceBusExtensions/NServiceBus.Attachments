using NServiceBus.Testing;

public class TestingOutgoing
{
    #region TestOutgoingHandler

    public class Handler :
        IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, HandlerContext context)
        {
            var options = new SendOptions();
            var attachments = options.Attachments();
            attachments.Add("theName", () => File.OpenRead("aFilePath"));
            return context.Send(new OtherMessage(), options);
        }
    }

    #endregion

    #region TestOutgoing

    [Fact]
    public async Task TestOutgoingAttachments()
    {
        //Arrange
        var context = new TestableMessageHandlerContext();
        var handler = new Handler();

        //Act
        await handler.Handle(new(), context);

        // Assert
        var sentMessage = context.SentMessages.Single();
        var attachments = sentMessage.Options.Attachments();
        var attachment = attachments.Items.Single();
        Assert.Contains("theName", attachment.Name);
        Assert.True(attachments.HasPendingAttachments);
    }

    #endregion
}