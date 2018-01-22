using System.Threading.Tasks;
using NServiceBus;

class MyHandler :
    IHandleMessages<SimpleMessage>
{
    public Task Handle(SimpleMessage message, IMessageHandlerContext context)
    {
        return Task.FromResult(0);
    }
}