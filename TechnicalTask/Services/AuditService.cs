using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;
using TechnicalTask.Entities;
using TechnicalTask.Repositories;
using TechnicalTask.Services.Interfaces;

namespace TechnicalTask.Services;

public sealed class AuditService : IAuditService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 200;

    private readonly IAuditRepository _auditRepository;

    public AuditService(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<PagedResponse<AuditResponse>> QueryAsync(AuditQueryRequest request, CancellationToken ct = default)
    {
        var pageNumber = NormalizePage(request.Page);
        var itemsPerPage = NormalizePageSize(request.PageSize);

        var query = new AuditQuery(
            request.BookId,
            request.FromUtc,
            request.ToUtc,
            request.Search,
            request.ChangeType,
            pageNumber,
            itemsPerPage,
            request.SortDirection
        );

        var queryResult = await _auditRepository.QueryAsync(query, ct);

        var items = queryResult.Items
            .Select(MapAudit)
            .ToList();

        return new PagedResponse<AuditResponse>(items, pageNumber, itemsPerPage, queryResult.TotalCount);
    }

    private static int NormalizePage(int page)
    {
        return page < DefaultPage ? DefaultPage : page;
    }

    private static int NormalizePageSize(int pageSize)
    {
        return pageSize is < 1 or > MaxPageSize ? DefaultPageSize : pageSize;
    }

    private static AuditResponse MapAudit(Audit audit)
    {
        return new AuditResponse(
            audit.Id,
            audit.BookId,
            audit.ChangeType.ToString(),
            audit.ChangedAt,
            audit.FieldName,
            audit.OldValue,
            audit.NewValue,
            audit.Description
        );
    }
}