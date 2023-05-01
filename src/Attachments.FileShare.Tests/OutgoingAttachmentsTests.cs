[UsesVerify]
public class OutgoingAttachmentsTests
{
    [Fact]
    public Task AttachmentToAdd()
    {
        var attachments = new OutgoingAttachments();
        attachments.Add(
            new()
            {
                Name = "Name",
                Stream = new MemoryStream()
            });
        return Verify(attachments.Inner);
    }
}