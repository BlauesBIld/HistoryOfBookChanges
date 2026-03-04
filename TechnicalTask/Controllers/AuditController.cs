using Microsoft.AspNetCore.Mvc;
using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;
using TechnicalTask.Services;

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<AuditResponse>>> Query([FromQuery] AuditQueryRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _auditService.QueryAsync(request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpGet("groups")]
    [ProducesResponseType(typeof(PagedResponse<AuditGroupResponse<AuditResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<AuditGroupResponse<AuditResponse>>>> QueryGrouped([FromQuery] AuditQueryRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _auditService.QueryGroupedAsync(request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }
}