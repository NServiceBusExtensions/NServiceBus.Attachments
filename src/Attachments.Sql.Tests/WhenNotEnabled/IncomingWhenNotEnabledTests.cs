using Microsoft.Extensions.DependencyInjection;

public class IncomingWhenNotEnabledTests : IDisposable
{
    public ManualResetEvent ResetEvent = new(false);
    public Exception? Exception;

    static IncomingWhenNotEnabledTests() =>
        DbSetup.Setup();

    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("SqlIncomingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.AssemblyScanner()
            .ExcludeAssemblies("xunit.runner.utility.netcoreapp10.dll");
        configuration.UseSerialization<SystemJsonSerializer>();
        configuration.RegisterComponents(_ => _.AddSingleton(this));
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

        public Handler(IncomingWhenNotEnabledTests incomingWhenNotEnabledTests) =>
            this.incomingWhenNotEnabledTests = incomingWhenNotEnabledTests;

        public Task Handle(SendMessage message, HandlerContext context)
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
        IMessage;

    public void Dispose() =>
        ResetEvent.Dispose();
}