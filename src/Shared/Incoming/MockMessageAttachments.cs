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
    /// <see cref="IMessageAttachments.CopyTo(string,Stream)"/>
    /// </summary>
    public virtual Task CopyTo(string name, Stream target) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(Stream)"/>
    /// </summary>
    public virtual Task CopyTo(Stream target) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(string,Func{AttachmentStream,Task})"/>
    /// </summary>
    public virtual Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream,Task})"/>
    /// </summary>
    public virtual Task ProcessStream(Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreams"/>
    /// </summary>
    public virtual Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// Read all attachment metadata for the current message.
    /// </summary>
    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        new AsyncEnumerable<AttachmentInfo>();

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes()"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes() =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream()"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream() =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(string)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(string name) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(string)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(string name) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(Encoding)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(Encoding? encoding) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(string,Encoding)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, string name, Stream target) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, Stream target) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{AttachmentStream,Task})"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{AttachmentStream,Task})"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{AttachmentStream,Task})"/>
    /// </summary>
    public virtual Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,string)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,string)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,Encoding)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,string,Encoding)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        Task.FromResult(AttachmentString.Empty);
}