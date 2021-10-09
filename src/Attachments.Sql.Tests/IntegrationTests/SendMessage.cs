using NServiceBus;

class SendMessage :
    IMessage
{
    public Guid MyId { get; set; } = Guid.NewGuid();
}