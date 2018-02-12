using System.Threading;

static class CancellationTokens
{
    public static CancellationToken? Or(this CancellationToken? token1, CancellationToken? token2)
    {
        if (token1 == null)
        {
            return token2;
        }

        return token1;
    }

    public static CancellationToken OrNone(this CancellationToken? token1, CancellationToken? token2)
    {
        return token1 ?? token2 ?? CancellationToken.None;
    }
}