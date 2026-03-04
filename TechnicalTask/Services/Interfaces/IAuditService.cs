using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;

namespace TechnicalTask.Services.Interfaces;

public interface IAuditService
{
    Task<PagedResponse<AuditResponse>> QueryAsync(AuditQueryRequest request, CancellationToken ct = default);
}