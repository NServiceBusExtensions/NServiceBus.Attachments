using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;

class Program
{
    static async Task Main()
    {
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }

        var configuration = new EndpointConfiguration("Attachments.Sql.Sample");
        configuration.EnableInstallers();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<MessageDrivenSubscriptions>();
        configuration.UsePersistence<LearningPersistence>();
        var transport = configuration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(Connection.ConnectionString);
        var attachments = configuration.EnableAttachments(Connection.ConnectionString, TimeToKeep.Default);
        attachments .UseTransportConnectivity();
        var endpoint = await Endpoint.Start(configuration);
        await SendMessage(endpoint);
        Console.WriteLine("Press any key to stop program");
        Console.ReadKey();
        await endpoint.Stop();
    }

    static async Task SendMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachments = sendOptions.Attachments();
        attachments.Add(
            name: "foo",
            streamFactory: () =>
            {
                var stream = new MemoryStream();
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write("content");
                streamWriter.Flush();
                stream.Position = 0;
                return Task.FromResult<Stream>(stream);
            });

        await endpoint.Send(new SendMessage(), sendOptions);
    }
}