using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;
using Xunit;

//TODO: add only send and only receive variants.
public class IntegrationTests
{
    static ManualResetEvent resetEvent = new ManualResetEvent(false);

    [Fact]
    public async Task Run()
    {
        SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);

        using (var sqlConnection = Connection.OpenConnection())
        {
            Installer.CreateTable(sqlConnection);
        }

        var configuration = new EndpointConfiguration("AttachmentsTest");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.EnableAttachments(
            connectionBuilder: Connection.NewConnection,
            timeToKeep: TimeToKeep.Default);
        var endpoint = await Endpoint.Start(configuration);
        await SendMessage(endpoint);
        resetEvent.WaitOne();
        await endpoint.Stop();
    }

    static async Task SendMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachments = sendOptions.OutgoingAttachments();
        attachments.Add(
            name: "foo",
            stream: GetStream);
        var attachment = sendOptions.OutgoingAttachment();
        attachment.Add(GetStream);


        await endpoint.Send(new MyMessage(), sendOptions);
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

    class Handler : IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                var incomingAttachments = context.IncomingAttachments();
                await incomingAttachments.CopyTo("foo", memoryStream);
                memoryStream.Position = 0;
                var buffer = memoryStream.GetBuffer();
                Debug.WriteLine(buffer);
            }
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

    class MyMessage : IMessage
    {
    }
}