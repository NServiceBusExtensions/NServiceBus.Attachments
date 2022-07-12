// ReSharper disable PartialTypeWithSinglePart

namespace NServiceBus.Attachments
#if FileShare
.FileShare.Testing
#endif
#if Sql
.Sql.Testing
#endif
;

/// <summary>
/// An implementation of <see cref="IMessageAttachments"/> for use in unit testing.
/// All members are stubbed out.
/// </summary>
/// <seealso cref="MockAttachmentHelper.InjectAttachmentsInstance"/>
public partial class MockMessageAttachments :
    IMessageAttachments
{
    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(string,Stream,CancellationToken)"/>
    /// </summary>
    public virtual Task CopyTo(string name, Stream target, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(Stream,CancellationToken)"/>
    /// </summary>
    public virtual Task CopyTo(Stream target, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(string,Func{AttachmentStream,Task},CancellationToken)"/>
    /// </summary>
    public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream,Task},CancellationToken)"/>
    /// </summary>
    public virtual Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreams"/>
    /// </summary>
    public virtual Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// Read all attachment metadata for the current message.
    /// </summary>
    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default) =>
        new AsyncEnumerable<AttachmentInfo>();

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(CancellationToken)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(CancellationToken cancellation = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(string,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(string,CancellationToken)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(string name, CancellationToken cancellation = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(Encoding,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(string,Encoding,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream,CancellationToken)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream,CancellationToken)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{AttachmentStream,Task},CancellationToken)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{AttachmentStream,Task},CancellationToken)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{AttachmentStream,Task},CancellationToken)"/>
    /// </summary>
    public virtual Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,CancellationToken)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,string,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,string,CancellationToken)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,Encoding,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,string,Encoding,CancellationToken)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default) =>
        Task.FromResult(AttachmentString.Empty);
}