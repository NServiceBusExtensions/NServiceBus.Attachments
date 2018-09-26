using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
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
    private static bool shouldPerformNestedConnection;

    static IntegrationTests()
    {
        DbSetup.Setup();
    }

    public IntegrationTests()
    {
        shouldPerformNestedConnection = true;
    }

    [Fact]
    public Task AdHoc()
    {
        return RunSql(
            useSqlTransport: false,
            useSqlTransportConnection: false,
            useSqlPersistence: false,
            useStorageSession: true,
            transactionMode: TransportTransactionMode.None);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task RunSql(bool useSqlTransport, bool useSqlTransportConnection,bool useSqlPersistence, bool useStorageSession, TransportTransactionMode transactionMode)
    {
        #if(NETCOREAPP)

        // sql persistence connection spans the handler. so a nested connection will cause DTC
        if (useSqlTransport && useSqlPersistence && !useStorageSession && transactionMode== TransportTransactionMode.TransactionScope)
        {
            // this scenario is not supported. since useStorageSession=false means attachments
            // will open a nested connection rather than use reuse the storage session connection
            //TODO: should detect this a runtime and throw  an better exception
            return;
        }
        if (useSqlPersistence && transactionMode == TransportTransactionMode.TransactionScope)
        {
            // so a nested connection will cause DTC
            shouldPerformNestedConnection = false;
        }
        #endif
        HandlerEvent = new ManualResetEvent(false);
        SagaEvent = new ManualResetEvent(false);
#if(NET472)
        var endpointName = "SqlIntegrationTestsNetClassic";
#else
        var endpointName = "SqlIntegrationTestsNetCore";
#endif
        var configuration = new EndpointConfiguration(endpointName);
        var attachments = configuration.EnableAttachments(Connection.ConnectionString, TimeToKeep.Default);
        if (useStorageSession)
        {
            attachments.UseSynchronizedStorageSessionConnectivity();
        }
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
        attachments.DisableCleanupTask();
#if(NET472)
        attachments.UseTable("AttachmentsNetClassic");
#else
        attachments.UseTable("AttachmentsNetCore");
#endif
        if (useSqlTransportConnection)
        {
            attachments.UseTransportConnectivity();
        }
        if (useSqlTransport)
        {
            var transport = configuration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(Connection.ConnectionString);
            transport.Transactions(transactionMode);
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

        var timeout = TimeSpan.FromSeconds(5);
        if (Debugger.IsAttached)
        {
            timeout = TimeSpan.FromSeconds(30);
        }
        if (!HandlerEvent.WaitOne(timeout))
        {
            throw new Exception("TimedOut");
        }
        if (!SagaEvent.WaitOne(timeout))
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
        if (shouldPerformNestedConnection)
        {
            using (var sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                sqlConnection.Open();
                Console.WriteLine(sqlConnection.ServerVersion);
            }
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