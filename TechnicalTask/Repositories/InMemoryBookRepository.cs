using System.Collections.Concurrent;
using TechnicalTask.Entities;

namespace TechnicalTask.Repositories;

public class InMemoryBookRepository : IBookRepository
{
    private readonly ConcurrentDictionary<Guid, Book> _books = new();

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _books.TryGetValue(id, out var book);
        return Task.FromResult(book);
    }

    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Book> result = _books.Values.ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(Book book, CancellationToken ct = default)
    {
        if (!_books.TryAdd(book.Id, book))
            throw new InvalidOperationException($"Book with id '{book.Id}' already exists.");

        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(Book book, CancellationToken ct = default)
    {
        if (!_books.ContainsKey(book.Id))
            return Task.FromResult(false);

        _books[book.Id] = book;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(_books.TryRemove(id, out _));
    }
}