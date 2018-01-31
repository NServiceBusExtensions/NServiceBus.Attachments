using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
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
        var configuration = new EndpointConfiguration("AttachmentsTest");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        //var recoverability = configuration.Recoverability();
        //recoverability.Immediate(x => x.NumberOfRetries(0));
        //recoverability.Delayed(x => x.NumberOfRetries(0));
        //configuration.DefineCriticalErrorAction(x =>
        //{
        //    return Task.CompletedTask;
        //});
        var endpoint = Endpoint.Start(configuration).Result;
        endpoint.SendLocal(new SendMessage()).Wait();
        resetEvent.WaitOne();
        endpoint.Stop().Wait();

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
}