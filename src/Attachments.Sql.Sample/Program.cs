using NServiceBus.Attachments.Sql;

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
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseSerialization<SystemJsonSerializer>();
        var transport = configuration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(Connection.ConnectionString);
        transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
        var attachments = configuration.EnableAttachments(Connection.NewConnection, TimeToKeep.Default);
        attachments.UseTransportConnectivity();
        var endpoint = await Endpoint.Start(configuration);
        await SendMessage(endpoint);
        Console.WriteLine("Press any key to stop program");
        Console.ReadKey();
        await endpoint.Stop();
    }

    static Task SendMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachments = sendOptions.Attachments();
        attachments.AddString(name: "foo", value: "content");
        return endpoint.Send(new SendMessage(), sendOptions);
    }
}