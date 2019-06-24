using System;
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
        transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
        var attachments = configuration.EnableAttachments(Connection.ConnectionString, TimeToKeep.Default);
        attachments.UseTransportConnectivity();
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
        attachments.AddString(name: "foo", value: "content");
        await endpoint.Send(new SendMessage(), sendOptions);
    }
}