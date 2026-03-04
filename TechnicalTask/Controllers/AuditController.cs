using Microsoft.AspNetCore.Mvc;
using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;
using TechnicalTask.Services.Interfaces;

namespace TechnicalTask.Controllers;

[ApiController]
[Route("api/audits")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AuditResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AuditResponse>>> Query(
        [FromQuery] AuditQueryRequest request,
        CancellationToken ct)
    {
        var result = await _auditService.QueryAsync(request, ct);
        return Ok(result);
    }
}