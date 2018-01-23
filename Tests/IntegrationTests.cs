using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;
using Xunit;

public class IntegrationTests
{
    const string connection = @"Data Source=.\SQLExpress;Database=NServiceBusAttachments; Integrated Security=True;Max Pool Size=100";
    static ManualResetEvent resetEvent = new ManualResetEvent(false);

    [Fact]
    public async Task Run()
    {
        SqlHelper.EnsureDatabaseExists(connection);

        var sqlConnection = new SqlConnection(connection);
        sqlConnection.Open();
        Installer.CreateTable(sqlConnection);
        var configuration = new EndpointConfiguration("AttachmentsTest");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.EnableAttachments(() => new SqlConnection(connection));
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
            stream: () =>
            {
                var stream = new MemoryStream();
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write("sdflgkndkjfgn");
                streamWriter.Flush();
                stream.Position = 0;
                return stream;
            },
            timeToKeep: before => TimeSpan.FromDays(1));

        await endpoint.Send(new MyMessage(), sendOptions);
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

            resetEvent.Set();
        }
    }
    class MyMessage : IMessage
    {
    }
}
