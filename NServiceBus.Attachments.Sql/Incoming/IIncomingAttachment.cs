using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public interface IIncomingAttachment
    {
        Task CopyTo(Stream target);
        Task ProcessStream(Func<Stream, Task> action);
        Task<byte[]> GetBytes();
    }
}