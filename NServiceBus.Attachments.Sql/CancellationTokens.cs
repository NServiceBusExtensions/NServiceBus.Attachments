using System.Threading;

static class CancellationTokens
{
    public static CancellationToken Or(this CancellationToken token1, CancellationToken token2)
    {
        if (token1 == CancellationToken.None)
        {
            return token2;
        }
        return token1;
    }
}