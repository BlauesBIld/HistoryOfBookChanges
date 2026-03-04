using System.Collections.Concurrent;

namespace TechnicalTask.Concurrency;

public sealed class BookLockProvider
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    public SemaphoreSlim Get(Guid bookId)
    {
        return _locks.GetOrAdd(bookId, _ => new SemaphoreSlim(1, 1));
    }
}