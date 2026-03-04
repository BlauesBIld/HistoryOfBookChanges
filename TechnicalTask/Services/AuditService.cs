using TechnicalTask.Common;
using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;
using TechnicalTask.Entities;
using TechnicalTask.Repositories;

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
            request.Search?.Trim(),
            request.ChangeType,
            pageNumber,
            itemsPerPage,
            request.SortDirection,
            AuditGroupBy.None
        );

        var queryResult = await _auditRepository.QueryAsync(query, ct);

        var items = queryResult.Items
            .Select(MapAudit)
            .ToList();

        return new PagedResponse<AuditResponse>(items, pageNumber, itemsPerPage, queryResult.TotalCount);
    }

    public async Task<PagedResponse<AuditGroupResponse<AuditResponse>>> QueryGroupedAsync(
        AuditQueryRequest request,
        CancellationToken ct = default)
    {
        if (request.GroupBy == AuditGroupBy.None)
            throw new ArgumentException("GroupBy must not be None for grouped audits.", nameof(request.GroupBy));

        var groupPage = NormalizePage(request.GroupPage);
        var groupsPerPage = NormalizePageSize(request.GroupPageSize);

        var itemPage = NormalizePage(request.Page);
        var itemsPerGroup = NormalizePageSize(request.PageSize);

        var query = new AuditQuery(
            request.BookId,
            request.FromUtc,
            request.ToUtc,
            request.Search?.Trim(),
            request.ChangeType,
            1,
            int.MaxValue,
            request.SortDirection,
            request.GroupBy
        );

        var allAudits = (await _auditRepository.QueryAsync(query, ct)).Items;

        var grouped = Group(allAudits, request.GroupBy)
            .Select(g =>
            {
                var pagedItems = g.Items
                    .Skip((itemPage - 1) * itemsPerGroup)
                    .Take(itemsPerGroup)
                    .Select(MapAudit)
                    .ToList();

                return new AuditGroupResponse<AuditResponse>(
                    g.Key,
                    pagedItems,
                    itemPage,
                    itemsPerGroup,
                    g.Items.Count
                );
            })
            .ToList();

        var totalGroups = grouped.Count;

        var pagedGroups = grouped
            .Skip((groupPage - 1) * groupsPerPage)
            .Take(groupsPerPage)
            .ToList();

        return new PagedResponse<AuditGroupResponse<AuditResponse>>(
            pagedGroups,
            groupPage,
            groupsPerPage,
            totalGroups
        );
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

    private static IReadOnlyList<GroupedAudits> Group(IReadOnlyList<Audit> audits, AuditGroupBy groupBy)
    {
        return groupBy switch
        {
            AuditGroupBy.BookId =>
                audits.GroupBy(a => a.BookId.ToString())
                    .Select(g => new GroupedAudits(g.Key, g.ToList()))
                    .ToList(),

            AuditGroupBy.ChangeType =>
                audits.GroupBy(a => a.ChangeType.ToString())
                    .Select(g => new GroupedAudits(g.Key, g.ToList()))
                    .ToList(),

            AuditGroupBy.FieldName =>
                audits.GroupBy(a => a.FieldName ?? "(none)")
                    .Select(g => new GroupedAudits(g.Key, g.ToList()))
                    .ToList(),

            AuditGroupBy.Day =>
                audits.GroupBy(a => a.ChangedAt.UtcDateTime.Date.ToString("yyyy-MM-dd"))
                    .Select(g => new GroupedAudits(g.Key, g.ToList()))
                    .ToList(),

            _ => throw new ArgumentOutOfRangeException(nameof(groupBy), groupBy, "Unsupported grouping")
        };
    }

    private sealed record GroupedAudits(string Key, List<Audit> Items);
}