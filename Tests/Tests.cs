using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NServiceBus;
using Xunit;

public class Tests
{
    //[Fact]
    //public async Task Simple_message_and_delegate_to_exclude()
    //{
    //    var message = new SimpleMessage();
    //    var result = await Send(message, _ => _.FilterAuditQueue(
    //        (instance, headers) => FilterResult.ExcludeFromAudit));
    //    Assert.True(result.Count == 0);
    //}

    static async Task<List<AuditedMessageData>> Send(object message, Action<EndpointConfiguration> addAuditFilter, [CallerMemberName] string key = null)
    {
        var testingTransport = new TestingTransport(key);
        var configuration = new EndpointConfiguration("AuditFilterSample");
        configuration.UsePersistence<LearningPersistence>();
        testingTransport.ApplyToEndpoint(configuration);
        addAuditFilter(configuration);

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        return await testingTransport.GetProcessedMessages(endpoint);
    }
}