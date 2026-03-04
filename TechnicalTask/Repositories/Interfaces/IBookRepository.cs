using TechnicalTask.Entities;

namespace TechnicalTask.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Book book, CancellationToken ct = default);
    Task<bool> UpdateAsync(Book book, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}