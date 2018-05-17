using System;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
    public partial class Persister : IPersister
    {
        string fullTableName;

        /// <summary>
        /// Instantiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(string schema = "dbo", string table = "MessageAttachments") :
            this(schema, table, true)
        {
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(string schema, string table, bool sanitize)
        {
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            Guard.AgainstNullOrEmpty(table, nameof(table));
            if (sanitize)
            {
                table = SqlSanitizer.Sanitize(table);
                schema = SqlSanitizer.Sanitize(schema);
            }

            fullTableName = $"{schema}.{table}";
        }

        static Exception ThrowNotFound(string messageId, string name)
        {
            return new Exception($"Could not find attachment. MessageId:{messageId}, Name:{name}");
        }
    }
}