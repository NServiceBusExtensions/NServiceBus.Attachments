using NServiceBus;

class SendMessage : IMessage
{
    public DataBusProperty<byte[]> Blob { get; set; }
}