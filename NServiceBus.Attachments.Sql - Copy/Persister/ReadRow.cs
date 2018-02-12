using System;

class ReadRow
{
    public readonly Guid Id;
    public readonly string MessageId;
    public readonly string Name;
    public readonly DateTime Expiry;

    public ReadRow(Guid id, string messageId, string name, DateTime expiry)
    {
        Id = id;
        MessageId = messageId;
        Name = name;
        Expiry = expiry;
    }
}