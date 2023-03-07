class AsyncEnumerable<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    IEnumerator<T>? inner;

    public AsyncEnumerable()
    {
    }
    public AsyncEnumerable(IEnumerable<T> inner) =>
        this.inner = inner.GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(Cancellation cancellation = default) =>
        this;

    public ValueTask DisposeAsync()
    {
        inner?.Dispose();
        return default;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        var moveNext = inner != null && inner.MoveNext();
        return new(moveNext);
    }

    public T Current
    {
        get
        {
            if (inner is not null)
            {
                return inner.Current;
            }

            throw new();
        }
    }
}