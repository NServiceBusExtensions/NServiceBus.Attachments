class MockAttachment
{
    public string Name;
    public byte[] Bytes;
    public IReadOnlyDictionary<string, string> Metadata;
    public DateTime Expiry;

    public MockAttachment(string name, byte[] bytes, IReadOnlyDictionary<string, string> metadata, DateTime expiry)
    {
        Name = name;
        Bytes = bytes;
        Metadata = metadata;
        Expiry = expiry;
    }
}