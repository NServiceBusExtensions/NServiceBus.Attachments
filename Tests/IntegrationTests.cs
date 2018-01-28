using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;
using Xunit;

public class IntegrationTests
{
    static ManualResetEvent resetEvent;

    static IntegrationTests()
    {
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }

        using (var sqlConnection = Connection.OpenConnection())
        {
            Installer.CreateTable(sqlConnection);
        }
    }

    [Fact]
    public async Task Run()
    {
        resetEvent = new ManualResetEvent(false);
        var configuration = GetEndpointConfiguration();
        var endpoint = await Endpoint.Start(configuration);
        await SendStartMessage(endpoint);
        resetEvent.WaitOne();
        await endpoint.Stop();
    }

    static EndpointConfiguration GetEndpointConfiguration(Action<AttachmentSettings> action = null)
    {
        var configuration = new EndpointConfiguration("AttachmentsTest");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        var attachments = configuration.EnableAttachments(Connection.OpenAsyncConnection, TimeToKeep.Default);
        action?.Invoke(attachments);
        return configuration;
    }

    static async Task SendStartMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.OutgoingAttachment();
        attachment.Add(GetStream);
        await endpoint.Send(new SendMessage(), sendOptions);
    }

    static Stream GetStream()
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write("sdflgkndkjfgn");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendHandler : IHandleMessages<SendMessage>
    {
        public Task Handle(SendMessage message, IMessageHandlerContext context)
        {
            var replyOptions = new ReplyOptions();
            var outgoingAttachment = replyOptions.OutgoingAttachment();
            outgoingAttachment.Add(() =>
            {
                var incomingAttachment = context.IncomingAttachment();
                return incomingAttachment.GetStream();
            });
            return context.Reply(new ReplyMessage(), replyOptions);
        }
    }

    class ReplyHandler : IHandleMessages<ReplyMessage>
    {
        public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                var incomingAttachment = context.IncomingAttachment();
                await incomingAttachment.CopyTo(memoryStream);
                memoryStream.Position = 0;
                var buffer = memoryStream.GetBuffer();
                Debug.WriteLine(buffer);
            }

            resetEvent.Set();
        }
    }

    class SendMessage : IMessage
    {
    }

    class ReplyMessage : IMessage
    {
    }
}