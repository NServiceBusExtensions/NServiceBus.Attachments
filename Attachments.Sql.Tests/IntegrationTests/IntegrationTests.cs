using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Features;
using NServiceBus.Persistence.Sql;
using Xunit;

public class IntegrationTests
{
    internal static ManualResetEvent HandlerEvent;
    internal static ManualResetEvent SagaEvent;

    static IntegrationTests()
    {
        DbSetup.Setup();
    }
    public class TestDataGenerator : IEnumerable<object[]>
    {
        List<TransportTransactionMode> transactionModes = new List<TransportTransactionMode>
        {
            TransportTransactionMode.None,
            TransportTransactionMode.ReceiveOnly,
            TransportTransactionMode.SendsAtomicWithReceive,
            TransportTransactionMode.TransactionScope,
        };
        List<bool> useSqlPersistenceList = new List<bool>
        {
            true,false
        };
        List<bool> useSqlTransportList = new List<bool>
        {
            true,false
        };
        List<bool> useSqlTransportConnectionList = new List<bool>
        {
            true,false
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var useSqlPersistence in useSqlPersistenceList)
            {
                foreach (var useSqlTransportConnection in useSqlTransportConnectionList)
                {
                    foreach (var useSqlTransport in useSqlTransportList)
                    {
                        foreach (var mode in transactionModes)
                        {
                            if (!useSqlTransport && mode == TransportTransactionMode.TransactionScope)
                            {
                                continue;
                            }

                            yield return new object[]
                            {
                                useSqlTransport,
                                useSqlTransportConnection,
                                useSqlPersistence,
                                mode
                            };
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    [Theory]
    [ClassData(typeof(TestDataGenerator))]

    public async Task RunSql(bool useSqlTransport, bool useSqlTransportConnection,bool useSqlPersistence, TransportTransactionMode transactionMode)
    {
        HandlerEvent = new ManualResetEvent(false);
        SagaEvent = new ManualResetEvent(false);
#if(NET472)
        var endpointName = "SqlIntegrationTestsNetClassic";
#else
        var endpointName = "SqlIntegrationTestsNetCore";
#endif
        var configuration = new EndpointConfiguration(endpointName);
        if (useSqlPersistence)
        {
            var persistence = configuration.UsePersistence<SqlPersistence>();
            Func<DbConnection> connectionBuilder = () => new SqlConnection(Connection.ConnectionString);
            await  RunSqlScripts(endpointName, connectionBuilder);
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.DisableInstaller();
            persistence.ConnectionBuilder(connectionBuilder);
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));
        }
        else
        {
            configuration.UsePersistence<LearningPersistence>();
        }

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
        if (!HandlerEvent.WaitOne(TimeSpan.FromSeconds(3)))
        {
            throw new Exception("TimedOut");
        }
        if (!SagaEvent.WaitOne(TimeSpan.FromSeconds(3)))
        {
            throw new Exception("TimedOut");
        }

        await endpoint.Stop();
    }

    static Task RunSqlScripts(string endpointName, Func<DbConnection> connectionBuilder)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var scriptDir = Path.Combine(baseDir, "NServiceBus.Persistence.Sql", "MsSqlServer");

        return ScriptRunner.Install(
                sqlDialect: new SqlDialect.MsSqlServer(),
                tablePrefix: endpointName+"_",
                connectionBuilder: connectionBuilder,
                scriptDirectory: scriptDir,
                shouldInstallOutbox: false,
                shouldInstallSagas: true,
                shouldInstallSubscriptions: false,
                shouldInstallTimeouts: false);
    }

    internal static void PerformNestedConnection()
    {
        using (var sqlConnection = new SqlConnection(Connection.ConnectionString))
        {
            sqlConnection.Open();
            Console.WriteLine(sqlConnection.ServerVersion);
        }
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
}