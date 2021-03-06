﻿using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class OutgoingWhenNotEnabledTests
{
    static OutgoingWhenNotEnabledTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public async Task Run()
    {
        EndpointConfiguration configuration = new("SqlOutgoingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = await Endpoint.Start(configuration);

        var exception = await Assert.ThrowsAsync<Exception>(() => SendStartMessageWithAttachment(endpoint));
        Assert.NotNull(exception);
        await Verifier.Verify(exception.Message);
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
        streamWriter.Write("sdflgkndkjfgn");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendMessage :
        IMessage
    {
    }
}