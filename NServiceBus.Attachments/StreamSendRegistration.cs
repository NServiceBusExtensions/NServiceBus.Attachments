﻿using System;
using System.Data.SqlClient;
using NServiceBus.Pipeline;

class StreamSendRegistration :
    RegisterStep
{
    public StreamSendRegistration(Func<SqlConnection> connectionBuilder)
        : base(
            stepId: "StreamSend",
            behavior: typeof(StreamSendBehavior),
            description: "Saves the payload into the shared location",
            factoryMethod: builder => new StreamSendBehavior(connectionBuilder))
    {
        InsertAfter("MutateOutgoingMessages");
        InsertBefore("ApplyTimeToBeReceived");
    }
}