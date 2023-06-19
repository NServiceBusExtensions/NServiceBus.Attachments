namespace NServiceBus.Attachments.FileShare.Testing;

public partial class MockMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancellation cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());
}