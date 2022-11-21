class MySaga :
    Saga<MySaga.SagaData>,
    IAmStartedByMessages<SendMessage>
{
    IntegrationTests integrationTests;

    public MySaga(IntegrationTests integrationTests) =>
        this.integrationTests = integrationTests;

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper) =>
        mapper.ConfigureMapping<SendMessage>(msg => msg.MyId)
            .ToSaga(saga => saga.MyId);

    public async Task Handle(SendMessage message, IMessageHandlerContext context)
    {
        var incomingAttachment = context.Attachments();
        using var stream = await incomingAttachment.GetStream();
        integrationTests.SagaEvent.Set();
    }

    public class SagaData :
        ContainSagaData
    {
        public Guid MyId { get; set; }
    }
}