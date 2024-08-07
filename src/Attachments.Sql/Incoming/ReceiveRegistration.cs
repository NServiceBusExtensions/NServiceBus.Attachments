﻿using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

class ReceiveRegistration(
    Func<Cancel, Task<SqlConnection>> connectionFactory,
    IPersister persister,
    bool useTransport,
    bool useSynchronizedStorage) :
    RegisterStep(stepId: $"{AssemblyHelper.Name}Receive",
        behavior: typeof(ReceiveBehavior),
        description: "Copies the shared data back to the logical messages",
        factoryMethod: _ => new ReceiveBehavior(connectionFactory, persister, useTransport, useSynchronizedStorage));