namespace NServiceBus.Attachments.Sql.Testing;

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

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(string,Func{AttachmentBytes,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(Func{AttachmentBytes,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrays"/>
    /// </summary>
    public virtual Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,string,Func{AttachmentBytes,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,Func{AttachmentBytes,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArraysForMessage(string,Func{AttachmentBytes,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

}