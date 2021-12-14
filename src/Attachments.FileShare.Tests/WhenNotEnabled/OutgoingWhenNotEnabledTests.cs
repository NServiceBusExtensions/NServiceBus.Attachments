﻿using NServiceBus;

[UsesVerify]
public class OutgoingWhenNotEnabledTests
{
    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("FileShareOutgoingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = await Endpoint.Start(configuration);

        var exception = await Assert.ThrowsAsync<Exception>(() => SendStartMessageWithAttachment(endpoint));
        await Verify(exception.Message);
        await endpoint.Stop();
    }

    static Task SendStartMessageWithAttachment(IEndpointInstance endpoint)
    {
        SendOptions sendOptions = new();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        return endpoint.Send(new SendMessage(), sendOptions);
    }

    static Stream GetStream()
    {
        MemoryStream stream = new();
        StreamWriter streamWriter = new(stream);
        streamWriter.Write("content");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendMessage :
        IMessage
    {
    }
}