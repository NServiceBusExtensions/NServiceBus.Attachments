using System;
using System.Threading.Tasks;
using NServiceBus;

class MySaga :
    Saga<MySaga.SagaData>,
    IAmStartedByMessages<SendMessage>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
    {
        mapper.ConfigureMapping<SendMessage>(msg => msg.MyId)
            .ToSaga(saga => saga.MyId);
    }

    public Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        IntegrationTests.SagaEvent.Set();
        return Task.CompletedTask;
    }

    public class SagaData :
        ContainSagaData
    {
        public Guid MyId { get; set; }
    }
}