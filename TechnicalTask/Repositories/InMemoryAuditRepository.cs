using System.Collections.Concurrent;
using TechnicalTask.Common;
using TechnicalTask.Entities;

namespace TechnicalTask.Repositories;

public sealed class InMemoryAuditRepository : IAuditRepository
{
    private readonly ConcurrentQueue<Audit> _audits = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Audit>> _auditsByBookId = new();

    public Task AddAsync(Audit entry, CancellationToken ct = default)
    {
        _audits.Enqueue(entry);

        var bookQueue = _auditsByBookId.GetOrAdd(entry.BookId, _ => new ConcurrentQueue<Audit>());
        bookQueue.Enqueue(entry);

        return Task.CompletedTask;
    }

    public Task<PagedResult<Audit>> QueryAsync(AuditQuery query, CancellationToken ct = default)
    {
        List<Audit> snapshot;

        if (query.BookId is not null && _auditsByBookId.TryGetValue(query.BookId.Value, out var queue))
            snapshot = queue.ToList();
        else
            snapshot = _audits.ToList();

        IEnumerable<Audit> filtered = snapshot;

        if (query.BookId is not null)
            filtered = filtered.Where(a => a.BookId == query.BookId);

        if (query.ChangeType is not null)
            filtered = filtered.Where(a => a.ChangeType == query.ChangeType.Value);

        if (query.FromUtc is not null)
            filtered = filtered.Where(a => a.ChangedAt >= query.FromUtc.Value);

        if (query.ToUtc is not null)
            filtered = filtered.Where(a => a.ChangedAt <= query.ToUtc.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            filtered = filtered.Where(a =>
                (a.Description?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.FieldName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.OldValue?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.NewValue?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        filtered = ApplySorting(filtered, query.SortDirection);

        var results = filtered.ToList();
        var totalCount = results.Count;

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;

        var items = results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Audit>(items, totalCount));
    }

    private static IEnumerable<Audit> ApplySorting(IEnumerable<Audit> source, SortDirection direction)
    {
        return direction == SortDirection.Descending
            ? source.OrderByDescending(a => a.ChangedAt)
            : source.OrderBy(a => a.ChangedAt);
    }
}