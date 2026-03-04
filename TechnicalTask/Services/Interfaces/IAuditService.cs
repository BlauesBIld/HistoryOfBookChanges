using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;

namespace TechnicalTask.Services;

public interface IAuditService
{
    Task<PagedResponse<AuditResponse>> QueryAsync(AuditQueryRequest request, CancellationToken ct = default);

    Task<PagedResponse<AuditGroupResponse<AuditResponse>>> QueryGroupedAsync(AuditQueryRequest request, CancellationToken ct = default);
}