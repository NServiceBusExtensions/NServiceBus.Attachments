using System;
using System.IO;
using NServiceBus.Attachments;

class OutgoingStream
{
    internal Func<Stream> Func;
    internal GetTimeToKeep TimeToKeep;
}