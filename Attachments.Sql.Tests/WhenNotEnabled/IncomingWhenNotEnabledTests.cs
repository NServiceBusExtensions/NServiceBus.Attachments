using System;
using System.Threading;
using System.Threading.Tasks;
#if(NET472)
using ApprovalTests;
#endif
using NServiceBus;
using Xunit;

public class IncomingWhenNotEnabledTests
{
    static ManualResetEvent resetEvent;
    static Exception exception;

    static IncomingWhenNotEnabledTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public void Run()
    {
        resetEvent = new ManualResetEvent(false);
        var configuration = new EndpointConfiguration("SqlIncomingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = Endpoint.Start(configuration).Result;
        endpoint.SendLocal(new SendMessage()).Wait();
        resetEvent.WaitOne();
        endpoint.Stop().Wait();

#if(NET472)
        Approvals.Verify(exception.Message);
#endif
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
}