public class TestDataGenerator :
    IEnumerable<object[]>
{
    List<TransportTransactionMode> transactionModes =
    [
        TransportTransactionMode.None,
        TransportTransactionMode.ReceiveOnly,
        TransportTransactionMode.SendsAtomicWithReceive,
        TransportTransactionMode.TransactionScope
    ];

    List<bool> useSqlPersistenceList =
    [
        true,
        false
    ];

    List<bool> runEarlyCleanupList =
    [
        true,
        false
    ];

    List<bool> useSqlTransportList =
    [
        true,
        false
    ];

    List<bool> useStorageSessionList =
    [
        true,
        false
    ];

    List<bool> useSqlTransportConnectionList =
    [
        true,
        false
    ];

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
                            foreach (var runEarlyCleanup in runEarlyCleanupList)
                            {
                                if (!useSqlTransport && mode != TransportTransactionMode.SendsAtomicWithReceive)
                                {
                                    continue;
                                }

                                yield return
                                [
                                    useSqlTransport,
                                    useSqlTransportConnection,
                                    useSqlPersistence,
                                    useStorageSession,
                                    mode,
                                    runEarlyCleanup
                                ];
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}