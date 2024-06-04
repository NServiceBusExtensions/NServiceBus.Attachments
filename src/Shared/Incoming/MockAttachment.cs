class MockAttachment(string name, byte[] bytes, IReadOnlyDictionary<string, string> metadata, DateTime expiry)
{
    public string Name = name;
    public byte[] Bytes = bytes;
    public IReadOnlyDictionary<string, string> Metadata = metadata;
    public DateTime Expiry = expiry;
}