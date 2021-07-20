using System;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
    public partial class Persister :
        IPersister
    {
        Table table;

        /// <summary>
        /// Instantiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(Table table)
        {
            this.table = table;
        }

        static Exception ThrowNotFound(string messageId, string name)
        {
            return new($"Could not find attachment. MessageId:{messageId}, Name:{name}");
        }
    }
}