using NServiceBus.Attachments.Sql;

[UsesVerify]
public class OutgoingAttachmentsTests
{
    [Fact]
    public Task AttachmentToAdd()
    {
        var attachments = new OutgoingAttachments();
        attachments.Add(
            new AttachmentToAdd
            {
                Name = "Name",
                Stream = new MemoryStream()
            });
        return Verify(attachments.Inner);
    }
}