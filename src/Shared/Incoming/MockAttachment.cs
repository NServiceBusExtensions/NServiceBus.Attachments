using System;
using System.Collections.Generic;

class MockAttachment
{
    public byte[] Bytes;
    public IReadOnlyDictionary<string, string> Metadata;
    public DateTime Expiry;

    public MockAttachment(byte[] bytes, IReadOnlyDictionary<string, string> metadata, DateTime expiry)
    {
        Bytes = bytes;
        Metadata = metadata;
        Expiry = expiry;
    }
}