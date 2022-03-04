using NServiceBus;

public class TestDataGenerator :
    IEnumerable<object[]>
{
    List<TransportTransactionMode> transactionModes = new()
    {
        TransportTransactionMode.None,
        TransportTransactionMode.ReceiveOnly,
        TransportTransactionMode.SendsAtomicWithReceive,
        TransportTransactionMode.TransactionScope
    };

    List<bool> useSqlPersistenceList = new()
    {
        true,
        false
    };

    List<bool> useSqlTransportList = new()
    {
        true,
        false
    };

    List<bool> useStorageSessionList = new()
    {
        true,
        false
    };

    List<bool> useSqlTransportConnectionList = new()
    {
        true,
        false
    };

    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var useSqlPersistence in useSqlPersistenceList)
        {
            foreach (var useSqlTransportConnection in useSqlTransportConnectionList)
            {
                foreach (var useStorageSession in useStorageSessionList)
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
                                useStorageSession,
                                mode
                            };
                        }
                    }
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}