using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public interface IMessageAttachments
    {
        Task CopyTo(string name, Stream target);
        Task CopyTo(Stream target);
        Task ProcessStream(string name, Func<Stream, Task> action);
        Task ProcessStream(Func<Stream, Task> action);
        Task ProcessStreams(Func<string, Stream, Task> action);
        Task<byte[]> GetBytes();
        Task<byte[]> GetBytes(string name);
        Task<Stream> GetStream();
        Task<Stream> GetStream(string name);
        Task CopyToForMessage(string messageId, string name, Stream target);
        Task CopyToForMessage(string messageId, Stream target);
        Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action);
        Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action);
        Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action);
        Task<byte[]> GetBytesForMessage(string messageId);
        Task<byte[]> GetBytesForMessage(string messageId, string name);
        Task<Stream> GetStreamForMessage(string messageId);
        Task<Stream> GetStreamForMessage(string messageId, string name);
    }
}