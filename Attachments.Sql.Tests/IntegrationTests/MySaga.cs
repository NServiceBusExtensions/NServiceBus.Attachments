using System;
using System.Diagnostics;
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

    public async Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var incomingAttachment = context.Attachments();
        using (var stream = await incomingAttachment.GetStream())
        {
            Debug.WriteLine(stream);
        }
        IntegrationTests.SagaEvent.Set();
    }

    public class SagaData :
        ContainSagaData
    {
        public Guid MyId { get; set; }
    }
}