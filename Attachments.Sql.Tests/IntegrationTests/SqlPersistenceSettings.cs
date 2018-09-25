using NServiceBus.Persistence.Sql;

[assembly: SqlPersistenceSettings(
    MsSqlServerScripts = true,
    MySqlScripts = false,
    OracleScripts = false,
    PostgreSqlScripts = false)]

