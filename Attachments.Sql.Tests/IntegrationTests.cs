using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using Xunit;

public class IntegrationTests
{
    static ManualResetEvent resetEvent;

    static IntegrationTests()
    {
        DbSetup.Setup();
    }

    [Theory]
    [InlineData(false, false, TransportTransactionMode.None)]
    [InlineData(false, false, TransportTransactionMode.ReceiveOnly)]
    [InlineData(false, false, TransportTransactionMode.SendsAtomicWithReceive)]
    [InlineData(true, false, TransportTransactionMode.None)]
    [InlineData(true, false, TransportTransactionMode.ReceiveOnly)]
    [InlineData(true, false, TransportTransactionMode.SendsAtomicWithReceive)]
    [InlineData(true, false, TransportTransactionMode.TransactionScope)]
    [InlineData(true, true, TransportTransactionMode.None)]
    [InlineData(true, true, TransportTransactionMode.ReceiveOnly)]
    [InlineData(true, true, TransportTransactionMode.SendsAtomicWithReceive)]
    [InlineData(true, true, TransportTransactionMode.TransactionScope)]
    public async Task RunSql(bool useSqlTransport, bool useSqlTransportConnection, TransportTransactionMode transactionMode)
    {
        resetEvent = new ManualResetEvent(false);
#if(NET472)
        var endpointName = "SqlIntegrationTestsNetClassic";
#else
        var endpointName = "SqlIntegrationTestsNetCore";
#endif
        var configuration = new EndpointConfiguration(endpointName);
        configuration.UsePersistence<LearningPersistence>();
        configuration.EnableInstallers();
        configuration.PurgeOnStartup(true);
        var attachments = configuration.EnableAttachments(Connection.ConnectionString, TimeToKeep.Default);
        attachments.DisableCleanupTask();
#if(NET472)
        attachments.UseTable("AttachmentsNetClassic");
#else
        attachments.UseTable("AttachmentsNetCore");
#endif
        if (useSqlTransport)
        {
            var transport = configuration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(Connection.ConnectionString);
            transport.Transactions(transactionMode);
            if (useSqlTransportConnection)
            {
                attachments.UseTransportConnectivity();
            }
        }
        else
        {
            var transport = configuration.UseTransport<LearningTransport>();
            transport.Transactions(transactionMode);
        }

        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<MessageDrivenSubscriptions>();
        var endpoint = await Endpoint.Start(configuration);
        await SendStartMessage(endpoint);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new Exception("TimedOut");
        }

        await endpoint.Stop();
    }

    static Task SendStartMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        attachment.Add("second", GetStream);
        attachment.Add("withMetadata", GetStream, metadata: new Dictionary<string, string> {{"key", "value"}});
        return endpoint.Send(new SendMessage(), sendOptions);
    }

    static Stream GetStream()
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write("content");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendHandler : IHandleMessages<SendMessage>
    {
        public async Task Handle(SendMessage message, IMessageHandlerContext context)
        {
            var replyOptions = new SendOptions();
            replyOptions.RouteToThisEndpoint();
            var attachment = await context.Attachments().GetBytes("withMetadata");
            Assert.Equal("value", attachment.Metadata["key"]);
            Assert.NotNull(attachment);
            var outgoingAttachment = replyOptions.Attachments();
            outgoingAttachment.AddBytes(attachment);

            using (var sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                await sqlConnection.OpenAsync();
                Console.WriteLine(sqlConnection.ServerVersion);
            }

            await context.Send(new ReplyMessage(), replyOptions);
        }
    }

    class ReplyHandler : IHandleMessages<ReplyMessage>
    {
        public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
        {
            var incomingAttachment = context.Attachments();

            using (var sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                await sqlConnection.OpenAsync();
                Console.WriteLine(sqlConnection.ServerVersion);
            }

            var buffer = incomingAttachment.GetBytes();
            Debug.WriteLine(buffer);
            resetEvent.Set();
        }
    }

    class SendMessage : IMessage
    {
    }

    class ReplyMessage : IMessage
    {
    }
}