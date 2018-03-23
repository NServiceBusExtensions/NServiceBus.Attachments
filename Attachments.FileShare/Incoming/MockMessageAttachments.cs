using System.IO;

namespace NServiceBus.Attachments.FileShare.Testing
{
    public partial class MockMessageAttachments
    {
        /// <summary>
        /// <see cref="IMessageAttachments.GetStream()"/>
        /// </summary>
        public virtual Stream GetStream()
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStream(string)"/>
        /// </summary>
        public virtual Stream GetStream(string name )
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string)"/>
        /// </summary>
        public virtual Stream GetStreamForMessage(string messageId)
        {
            return null;
        }

        /// <summary>
        /// <see cref="IMessageAttachments.GetStreamForMessage(string,string)"/>
        /// </summary>
        public virtual Stream GetStreamForMessage(string messageId, string name)
        {
            return null;
        }
    }
}