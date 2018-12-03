using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;

class ReplyHandler : IHandleMessages<ReplyMessage>
{
    public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
    {
        var randomFileName = Path.GetRandomFileName();
        var incomingAttachment = context.Attachments();
        using (var target = File.Create(randomFileName))
        {
            await incomingAttachment.CopyTo(target).ConfigureAwait(false);
        }
        File.Delete(randomFileName);

        AttachmentsRunner.countdownEvent.Signal();
        Console.WriteLine(AttachmentsRunner.countdownEvent.CurrentCount);
    }
}