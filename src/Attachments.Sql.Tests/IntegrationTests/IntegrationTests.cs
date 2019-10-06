using System;
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
using Xunit.Abstractions;

public class IntegrationTests :
    XunitApprovalBase
{
    internal static ManualResetEvent HandlerEvent = null!;
    internal static ManualResetEvent SagaEvent = null!;
    bool shouldPerformNestedConnection = true;

    static IntegrationTests()
    {
        DbSetup.Setup();
    }

    [Fact]
    public Task AdHoc()
    {
        return RunSql(
            useSqlTransport: false,
            useSqlTransportConnection: false,
            useSqlPersistence: false,
            useStorageSession: false,
            transactionMode: TransportTransactionMode.SendsAtomicWithReceive);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task RunSql(bool useSqlTransport, bool useSqlTransportConnection, bool useSqlPersistence, bool useStorageSession, TransportTransactionMode transactionMode)
    {
        // sql persistence connection spans the handler. so a nested connection will cause DTC
        if (useSqlTransport && useSqlPersistence && !useStorageSession && transactionMode == TransportTransactionMode.TransactionScope)
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
        HandlerEvent = new ManualResetEvent(false);
        SagaEvent = new ManualResetEvent(false);
        var endpointName = "SqlIntegrationTests";
        var configuration = new EndpointConfiguration(endpointName);
        var attachments = configuration.EnableAttachments(() => Connection.OpenAsyncConnection(), TimeToKeep.Default);
        if (useStorageSession)
        {
            attachments.UseSynchronizedStorageSessionConnectivity();
        }
        configuration.RegisterComponents(
            registration: configureComponents =>
            {
                configureComponents.RegisterSingleton(this);
            });
        if (useSqlPersistence)
        {
            var persistence = configuration.UsePersistence<SqlPersistence>();
            Func<DbConnection> connectionBuilder = () => new SqlConnection(Connection.ConnectionString);
            await RunSqlScripts(endpointName, connectionBuilder);
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
        attachments.UseTable("Attachments");
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

        var timeout = TimeSpan.FromSeconds(30);
        if (!HandlerEvent.WaitOne(timeout))
        {
            throw new Exception("TimedOut");
        }

        if (!SagaEvent.WaitOne(timeout))
        {
            throw new Exception("TimedOut");
        }

        await endpoint.Stop();
        base.Dispose();
    }

    static Task RunSqlScripts(string endpointName, Func<DbConnection> connectionBuilder)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var scriptDir = Path.Combine(baseDir, "NServiceBus.Persistence.Sql", "MsSqlServer");

        return ScriptRunner.Install(
            sqlDialect: new SqlDialect.MsSqlServer(),
            tablePrefix: endpointName + "_",
            connectionBuilder: connectionBuilder,
            scriptDirectory: scriptDir,
            shouldInstallOutbox: false,
            shouldInstallSagas: true,
            shouldInstallSubscriptions: false,
            shouldInstallTimeouts: false);
    }

    internal void PerformNestedConnection()
    {
        if (shouldPerformNestedConnection)
        {
            using (var connection = new SqlConnection(Connection.ConnectionString))
            {
                connection.Open();
                Console.WriteLine(connection.ServerVersion);
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

    public IntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }

    public override void Dispose()
    {
    }
}