using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public interface IOutgoingAttachments
    {
        void Add<T>(string name, Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream;
        void Add(string name, Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null);
        void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null);
    }
}