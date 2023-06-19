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
    public Cancellation Cancellation { get; }

    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(string,Stream,Cancellation)"/>
    /// </summary>
    public virtual Task CopyTo(string name, Stream target, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(Stream,Cancellation)"/>
    /// </summary>
    public virtual Task CopyTo(Stream target, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(string,Func{AttachmentStream,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreams"/>
    /// </summary>
    public virtual Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// Read all attachment metadata for the current message.
    /// </summary>
    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancel = default) =>
        new AsyncEnumerable<AttachmentInfo>();

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(Cancellation cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(Cancellation)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(Cancellation cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(string,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(string name, Cancellation cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(string,Cancellation)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(string name, Cancellation cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(Encoding,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(string,Encoding,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream,Cancellation)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream,Cancellation)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, Stream target, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{AttachmentStream,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{AttachmentStream,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{AttachmentStream,Cancellation,Task},Cancellation)"/>
    /// </summary>
    public virtual Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,Cancellation)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,string,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,string,Cancellation)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,Encoding,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,string,Encoding,Cancellation)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancel = default) =>
        Task.FromResult(AttachmentString.Empty);
}