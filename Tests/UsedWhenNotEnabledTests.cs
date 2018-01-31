using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;
using Xunit;

public class UsedWhenNotEnabledTests
{
    static UsedWhenNotEnabledTests()
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
        var configuration = new EndpointConfiguration("AttachmentsTest");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = await Endpoint.Start(configuration);

        var exception = await Assert.ThrowsAsync<Exception>(() => SendStartMessage(endpoint));
        Assert.Equal(UsedWhenNotEnabledBehavior.Text, exception.Message);
        await endpoint.Stop();
    }

    static async Task SendStartMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
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

    class SendMessage : IMessage
    {
    }
}