using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;

class Program
{
    const string connection = @"Data Source=.\SQLExpress;Database=NServiceBusAttachments; Integrated Security=True;Max Pool Size=100";
    static async Task Main()
    {
        SqlHelper.EnsureDatabaseExists(connection);
        var configuration = new EndpointConfiguration("AttachmentsSample");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.AuditProcessedMessagesTo("audit");
        configuration.EnableAttachments(BuildSqlConnection);
        Installer.CreateTable(OpenConnection());
        var endpoint = await Endpoint.Start(configuration);
        await SendMessage(endpoint);
        Console.WriteLine("Press any key to stop program");
        Console.Read();
        await endpoint.Stop();
    }

    static SqlConnection OpenConnection()
    {
        var connection = BuildSqlConnection();
        connection.Open();
        return connection;
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

    static SqlConnection BuildSqlConnection()
    {
        return new SqlConnection(connection);
    }
}