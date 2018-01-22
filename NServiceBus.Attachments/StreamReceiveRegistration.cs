using NServiceBus.Attachments;
using NServiceBus.Pipeline;

class StreamReceiveRegistration :
    RegisterStep
{
    public StreamReceiveRegistration(AttachmentSettings attachmentSettings)
        : base(
            stepId: "StreamReceive",
            behavior: typeof(StreamReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: builder => new StreamReceiveBehavior(attachmentSettings.GetConnectionAndTransaction()))
    {

    }
}