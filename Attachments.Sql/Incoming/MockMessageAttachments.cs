using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql.Testing
{
    public partial class MockMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(CancellationToken)"/>
        /// </summary>
        public virtual Task<Stream> GetStream(CancellationToken cancellation = default)
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<Stream> GetStream(string name, CancellationToken cancellation = default )
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,CancellationToken)"/>
        /// </summary>
        public virtual Task<Stream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
        {
            return Task.FromResult<Stream>(null);
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string,CancellationToken)"/>
        /// </summary>
        public virtual Task<Stream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
        {
            return Task.FromResult<Stream>(null);
        }
    }
}