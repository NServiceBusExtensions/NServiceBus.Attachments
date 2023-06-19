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
    /// <see cref="IMessageAttachments.CopyTo(string,Stream,Cancel)"/>
    /// </summary>
    public virtual Task CopyTo(string name, Stream target, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyTo(Stream,Cancel)"/>
    /// </summary>
    public virtual Task CopyTo(Stream target, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(string,Func{AttachmentStream,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessStream(string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStream(Func{AttachmentStream,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessStream(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreams"/>
    /// </summary>
    public virtual Task ProcessStreams(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// Read all attachment metadata for the current message.
    /// </summary>
    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancel cancel = default) =>
        new AsyncEnumerable<AttachmentInfo>();

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(Cancel cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(Cancel)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(Cancel cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytes(string,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytes(string name, Cancel cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStream(string,Cancel)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStream(string name, Cancel cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(Encoding,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(Encoding? encoding, Cancel cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetString(string,Encoding,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetString(string name, Encoding? encoding, Cancel cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,string,Stream,Cancel)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, string name, Stream target, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.CopyToForMessage(string,Stream,Cancel)"/>
    /// </summary>
    public virtual Task CopyToForMessage(string messageId, Stream target, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,string,Func{AttachmentStream,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamForMessage(string,Func{AttachmentStream,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.ProcessStreamsForMessage(string,Func{AttachmentStream,Cancel,Task},Cancel)"/>
    /// </summary>
    public virtual Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        Task.CompletedTask;

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancel cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,Cancel)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancel cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetBytesForMessage(string,string,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancel cancel = default) =>
        Task.FromResult(AttachmentBytes.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetMemoryStreamForMessage(string,string,Cancel)"/>
    /// </summary>
    public virtual Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        Task.FromResult(new MemoryStream());

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,Encoding,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancel cancel = default) =>
        Task.FromResult(AttachmentString.Empty);

    /// <summary>
    /// <see cref="IMessageAttachments.GetStringForMessage(string,string,Encoding,Cancel)"/>
    /// </summary>
    public virtual Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancel cancel = default) =>
        Task.FromResult(AttachmentString.Empty);
}