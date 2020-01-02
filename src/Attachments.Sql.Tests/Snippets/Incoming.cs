using System.IO;
using System.Threading.Tasks;
using NServiceBus;

public class Incoming
{
    #region ProcessStream

    class HandlerProcessStream :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
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
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var attachments = context.Attachments();
            await attachments.ProcessStreams(
                    action: async stream =>
                    {
                        // Use the attachment stream. in this example copy to a file
                        await using var fileToCopyTo = File.Create($"{stream.Name}.txt");
                        await stream.CopyToAsync(fileToCopyTo);
                    })
                .ConfigureAwait(false);
        }
    }

    #endregion

    #region ProcessStreamsForMessage

    class HandlerProcessStreamsForMessage :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var attachments = context.Attachments();
            await attachments.ProcessStreamsForMessage(
                    messageId: "theMessageId",
                    action: async stream =>
                    {
                        // Use the attachment stream. in this example copy to a file
                        await using var fileToCopyTo = File.Create($"{stream.Name}.txt");
                        await stream.CopyToAsync(fileToCopyTo);
                    })
                .ConfigureAwait(false);
        }
    }

    #endregion

    #region CopyTo

    class HandlerCopyTo :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var attachments = context.Attachments();
            using var fileToCopyTo = File.Create("FilePath.txt");
            await attachments.CopyTo("attachment1", fileToCopyTo);
        }
    }

    #endregion

    #region GetBytes

    class HandlerGetBytes :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
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
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var attachments = context.Attachments();
            await using var attachment = await attachments.GetStream("attachment1");
            // Use the attachment stream. in this example copy to a file
            await using var fileToCopyTo = File.Create("FilePath.txt");
            await attachment.CopyToAsync(fileToCopyTo);
        }
    }

    #endregion
}