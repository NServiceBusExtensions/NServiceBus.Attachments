﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    public class OutgoingAttachment
    {
        OutgoingAttachments attachments;

        public OutgoingAttachment(OutgoingAttachments attachments)
        {
            this.attachments = attachments;
        }

        public void Add<T>(Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
        {
            attachments.Add(string.Empty, stream, timeToKeep, cleanup);
        }

        public void Add(Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        {
            attachments.Add(string.Empty, stream, timeToKeep, cleanup);
        }
    }
}