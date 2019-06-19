using System;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
using NServiceBus;
using Xunit;
using Xunit.Abstractions;

public class IncomingWhenNotEnabledTests :
    TestBase
{
    static ManualResetEvent resetEvent;
    static Exception exception;

    static IncomingWhenNotEnabledTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public async Task Run()
    {
        resetEvent = new ManualResetEvent(false);
        var configuration = new EndpointConfiguration("SqlIncomingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(new SendMessage());
        resetEvent.WaitOne();
        await endpoint.Stop();

        Approvals.Verify(exception.Message);
    }

    class Handler : IHandleMessages<SendMessage>
    {
        public Task Handle(SendMessage message, IMessageHandlerContext context)
        {
            try
            {
                context.Attachments();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                resetEvent.Set();
            }

            return Task.CompletedTask;
        }
    }

    class SendMessage : IMessage
    {
    }

    public IncomingWhenNotEnabledTests(ITestOutputHelper output) :
        base(output)
    {
    }
}