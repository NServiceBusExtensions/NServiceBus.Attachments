// ReSharper disable UnusedVariable

public class Incoming
{
    #region ProcessStream

    class HandlerProcessStream :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            await attachments.ProcessStream(
                name: "attachment1",
                action: async stream =>
                {
                    // Use the attachment stream. in this example copy to a file
                    await using var fileToCopyTo = File.Create("FilePath.txt");
                    await stream.CopyToAsync(fileToCopyTo);
                });
        }
    }

    #endregion

    #region ProcessStreams

    class HandlerProcessStreams :
        IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            return attachments.ProcessStreams(
                action: async stream =>
                {
                    // Use the attachment stream. in this example copy to a file
                    await using var file = File.Create($"{stream.Name}.txt");
                    await stream.CopyToAsync(file);
                });
        }
    }

    #endregion

    #region ProcessStreamsForMessage

    class HandlerProcessStreamsForMessage :
        IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            return attachments.ProcessStreamsForMessage(
                messageId: "theMessageId",
                action: async stream =>
                {
                    // Use the attachment stream. in this example copy to a file
                    await using var toCopyTo = File.Create($"{stream.Name}.txt");
                    await stream.CopyToAsync(toCopyTo);
                });
        }
    }

    #endregion

    #region CopyTo

    class HandlerCopyTo :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            await using var fileToCopyTo = File.Create("FilePath.txt");
            await attachments.CopyTo("attachment1", fileToCopyTo);
        }
    }

    #endregion

    #region GetBytes

    class HandlerGetBytes :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            var bytes = await attachments.GetBytes("attachment1");
            // use the byte array
        }
    }

    #endregion

    #region GetStream

    class HandlerGetStream :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, HandlerContext context)
        {
            var attachments = context.Attachments();
            await using var stream = await attachments.GetStream("attachment1");
            // Use the attachment stream. in this example copy to a file
            await using var fileToCopyTo = File.Create("FilePath.txt");
            await stream.CopyToAsync(fileToCopyTo);
        }
    }

    #endregion
}