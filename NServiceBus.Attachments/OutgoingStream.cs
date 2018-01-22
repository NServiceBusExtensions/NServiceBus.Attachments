using System;
using System.IO;

namespace NServiceBus.Attachments
{
    class OutgoingStream
    {
        internal Func<Stream> Func;
        internal GetTimeToKeep TimeToKeep;
    }
}