using System;
using System.Collections.Generic;

class MockAttachment
{
    public byte[] Bytes;
    public IReadOnlyDictionary<string, string> Metadata;
    public DateTime Expiry;
}