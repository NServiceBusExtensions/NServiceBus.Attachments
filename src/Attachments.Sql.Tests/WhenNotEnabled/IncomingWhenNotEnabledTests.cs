using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class IncomingWhenNotEnabledTests :
    VerifyBase
{
    public ManualResetEvent ResetEvent = new ManualResetEvent(false);
    public Exception? Exception;

    static IncomingWhenNotEnabledTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("SqlIncomingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.RegisterComponents(components => components.RegisterSingleton(this));
        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(new SendMessage());
        ResetEvent.WaitOne();
        await endpoint.Stop();

        Assert.NotNull(Exception);
        await Verify(Exception!.Message);
    }

    class Handler :
        IHandleMessages<SendMessage>
    {
        IncomingWhenNotEnabledTests incomingWhenNotEnabledTests;

        public Handler(IncomingWhenNotEnabledTests incomingWhenNotEnabledTests)
        {
            this.incomingWhenNotEnabledTests = incomingWhenNotEnabledTests;
        }
        public Task Handle(SendMessage message, IMessageHandlerContext context)
        {
            try
            {
                context.Attachments();
            }
            catch (Exception e)
            {
                incomingWhenNotEnabledTests.Exception = e;
            }
            finally
            {
                incomingWhenNotEnabledTests.ResetEvent.Set();
            }

            return Task.CompletedTask;
        }
    }

    class SendMessage :
        IMessage
    {
    }

    public override void Dispose()
    {
        ResetEvent.Dispose();
        base.Dispose();
    }

    public IncomingWhenNotEnabledTests(ITestOutputHelper output) :
        base(output)
    {
    }
}