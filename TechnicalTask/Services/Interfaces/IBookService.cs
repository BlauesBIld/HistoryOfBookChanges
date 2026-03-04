using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;

namespace TechnicalTask.Services.Interfaces;

public interface IBookService
{
    Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
    Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BookResponse?> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}