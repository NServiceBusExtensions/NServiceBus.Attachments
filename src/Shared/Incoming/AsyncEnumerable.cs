using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class AsyncEnumerable<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    IEnumerator<T>? inner;

    public AsyncEnumerable()
    {
    }
    public AsyncEnumerable(IEnumerable<T> inner)
    {
        this.inner = inner.GetEnumerator();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return this;
    }

    public ValueTask DisposeAsync()
    {
        inner?.Dispose();
        return default;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        var moveNext = inner != null && inner.MoveNext();
        return new ValueTask<bool>(moveNext);
    }

    public T Current
    {
        get
        {
            if (inner != null)
            {
                return inner.Current;
            }

            throw new();
        }
    }
}