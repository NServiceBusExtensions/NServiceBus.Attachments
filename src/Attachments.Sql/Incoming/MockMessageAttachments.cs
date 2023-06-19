namespace NServiceBus.Attachments.Sql.Testing;

public partial class MockMessageAttachments
{
    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(Cancel cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string name, Cancel cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, Cancel cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        Task.FromResult(AttachmentStream.Empty());

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(string,Func{AttachmentBytes,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(Func{AttachmentBytes,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrays"/>
    /// </summary>
    public virtual Task ProcessByteArrays(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,string,Func{AttachmentBytes,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,Func{AttachmentBytes,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArraysForMessage(string,Func{AttachmentBytes,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

}