using TechnicalTask.Common;
using TechnicalTask.Entities;

namespace TechnicalTask.Repositories;

public interface IAuditRepository
{
    Task AddAsync(Audit entry, CancellationToken ct = default);
    Task<PagedResult<Audit>> QueryAsync(AuditQuery query, CancellationToken ct = default);
}