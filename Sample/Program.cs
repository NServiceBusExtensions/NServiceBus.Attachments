using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;

class Program
{
    static async Task Main()
    {
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }
        var configuration = new EndpointConfiguration("AttachmentsSample");
        configuration.EnableInstallers();
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.AuditProcessedMessagesTo("audit");
        configuration.EnableAttachments(Connection.OpenAsyncConnection,TimeToKeep.Default);
        var endpoint = await Endpoint.Start(configuration);
        await SendMessage(endpoint);
        Console.WriteLine("Press any key to stop program");
        Console.Read();
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
                return Task.FromResult<Stream>(stream);
            });

        await endpoint.Send(new MyMessage(), sendOptions);
    }
}