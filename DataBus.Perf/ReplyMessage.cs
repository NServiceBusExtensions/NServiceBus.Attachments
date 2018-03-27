using NServiceBus;

class ReplyMessage : IMessage
{
    public DataBusProperty<byte[]> Blob { get; set; }
}