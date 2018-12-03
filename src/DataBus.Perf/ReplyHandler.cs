using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;

class ReplyHandler : IHandleMessages<ReplyMessage>
{
    public Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var randomFileName = Path.GetRandomFileName();
        File.WriteAllBytes(randomFileName, message.Blob.Value);
        File.Delete(randomFileName);

        AttachmentsRunner.countdownEvent.Signal();
        Console.WriteLine(AttachmentsRunner.countdownEvent.CurrentCount);
        return Task.CompletedTask;
    }
}