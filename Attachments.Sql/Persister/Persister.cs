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
        public Persister(string schema = "dbo", string tableName = "MessageAttachments") :
            this(schema, tableName, true)
        {
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(string schema, string tableName, bool sanitize)
        {
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            if (sanitize)
            {
                tableName = SqlSanitizer.Sanitize(tableName);
                schema = SqlSanitizer.Sanitize(schema);
            }

            fullTableName = $"{schema}.{tableName}";
        }

        static Exception ThrowNotFound(string messageId, string name)
        {
            return new Exception($"Could not find attachment. MessageId:{messageId}, Name:{name}");
        }
    }
}