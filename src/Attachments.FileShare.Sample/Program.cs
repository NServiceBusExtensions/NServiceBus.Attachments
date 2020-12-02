using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.FileShare;

if (!Connection.IsUsingEnvironmentVariable)
{
    SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
}

EndpointConfiguration configuration = new("Attachments.FileShare.Sample");
configuration.EnableInstallers();
configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();
configuration.AuditProcessedMessagesTo("audit");
configuration.EnableAttachments("Attachments", TimeToKeep.Default);
var endpoint = await Endpoint.Start(configuration);
await SendMessage(endpoint);
Console.WriteLine("Press any key to stop program");
Console.ReadKey();
await endpoint.Stop();

static Task SendMessage(IEndpointInstance endpoint)
{
    SendOptions sendOptions = new();
    sendOptions.RouteToThisEndpoint();
    var attachments = sendOptions.Attachments();
    attachments.AddString(name: "foo", value: "content");
    return endpoint.Send(new MyMessage(), sendOptions);
}
