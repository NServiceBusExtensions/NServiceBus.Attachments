﻿using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class StreamPersisterTests: TestBase
{
    StreamPersister persister;

    static StreamPersisterTests()
    {
        if (!Connection.IsUsingEnvironmentVariable)
        {
            SqlHelper.EnsureDatabaseExists(Connection.ConnectionString);
        }
    }

    public StreamPersisterTests(ITestOutputHelper output) : base(output)
    {
        persister = new StreamPersister("dbo", "Attachments");
    }

    [Fact]
    public async Task CopyTo()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection);
            persister.DeleteAllRows(connection);
            await persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000,1,1,1,1,1), GetStream());
            var memoryStream = new MemoryStream();
            await persister.CopyTo("theMessageId", "theName", connection, memoryStream);

            memoryStream.Position = 0;
            Assert.Equal(5, memoryStream.GetBuffer()[0]);
        }
    }

    [Fact]
    public async Task ProcessStream()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection);
            persister.DeleteAllRows(connection);
            await persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000,1,1,1,1,1), GetStream());
            await persister.ProcessStream("theMessageId", "theName", connection,
                action: stream =>
                {
                    var array = GetBytes(stream);
                    Assert.Equal(5, array[0]);
                    return Task.CompletedTask;
                });
        }
    }

    static byte[] GetBytes(Stream stream)
    {
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    [Fact]
    public void SaveStream()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection);
            persister.DeleteAllRows(connection);
            persister.SaveStream(connection, null, "theMessageId", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            ObjectApprover.VerifyWithJson(persister.ReadAllMetadata(connection));
        }
    }
    [Fact]
    public void CleanupItemsOlderThan()
    {
        using (var connection = Connection.OpenConnection())
        {
            Installer.CreateTable(connection);
            persister.DeleteAllRows(connection);
            persister.SaveStream(connection, null, "theMessageId1", "theName", new DateTime(2000, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            persister.SaveStream(connection, null, "theMessageId2", "theName", new DateTime(2002, 1, 1, 1, 1, 1), GetStream()).GetAwaiter().GetResult();
            persister.CleanupItemsOlderThan(connection, new DateTime(2001, 1, 1, 1, 1, 1));
            ObjectApprover.VerifyWithJson(persister.ReadAllMetadata(connection));
        }
    }

    Stream GetStream()
    {
        var stream = new MemoryStream();
        stream.WriteByte(5);
        stream.Position = 0;
        return stream;
    }

}