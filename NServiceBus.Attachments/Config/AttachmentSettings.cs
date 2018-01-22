using System;
using System.Data.SqlClient;

namespace NServiceBus
{
    class AttachmentSettings
    {
        public readonly Func<SqlConnection> ConnectionBuilder;

        public AttachmentSettings(Func<SqlConnection> connectionBuilder)
        {
            ConnectionBuilder = connectionBuilder;
        }
    }
}