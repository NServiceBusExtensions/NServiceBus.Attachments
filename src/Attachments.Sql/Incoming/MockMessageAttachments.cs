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

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(string,Func{AttachmentBytes,Task})"/>
    /// </summary>
    public virtual Task ProcessByteArray(string name, Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArray(Func{AttachmentBytes,Task})"/>
    /// </summary>
    public virtual Task ProcessByteArray(Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrays"/>
    /// </summary>
    public virtual Task ProcessByteArrays(Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,string,Func{AttachmentBytes,Task})"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArrayForMessage(string,Func{AttachmentBytes,Task})"/>
    /// </summary>
    public virtual Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessByteArraysForMessage(string,Func{AttachmentBytes,Task})"/>
    /// </summary>
    public virtual Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Task> action) =>
        Task.CompletedTask;

}