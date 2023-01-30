namespace NServiceBus.Attachments.Sql.Testing;

public partial class MockMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream() =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        Task.FromResult(AttachmentStream.Empty());
}