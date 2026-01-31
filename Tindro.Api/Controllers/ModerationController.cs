﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Application.Moderation.Dtos;
using Tindro.Application.Moderation.Interfaces;


[Authorize]
[ApiController]
[Route("api/v1/moderation")]
public class ModerationController : ControllerBase
{
    private readonly IModerationService _moderationService;

    public ModerationController(IModerationService moderationService)
    {
        _moderationService = moderationService;
    }

    /// <summary>
    /// Reports a user for inappropriate behavior or content.
    /// </summary>
    [HttpPost("report")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> ReportUser([FromBody] CreateReportRequestDto request)
    {
        var reporterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var report = await _moderationService.CreateReportAsync(request, reporterId);
        return CreatedAtAction(nameof(GetReportById), new { reportId = report.Id }, report);
    }

    /// <summary>
    /// [Admin] Gets a paginated list of reports.
    /// </summary>
    [HttpGet("reports")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(typeof(PagedResult<ReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] ReportQueryParameters query)
    {
        var reports = await _moderationService.GetReportsAsync(query);
        return Ok(reports);
    }

    /// <summary>
    /// [Admin] Gets a single report by its ID.
    /// </summary>
    [HttpGet("reports/{reportId}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportById(Guid reportId)
    {
        var report = await _moderationService.GetReportByIdAsync(reportId);
        return Ok(report);
    }

    /// <summary>

    /// </summary>
    [HttpPost("reports/{reportId}/resolve")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResolveReport(Guid reportId, [FromBody] ResolveReportRequestDto request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _moderationService.ResolveReportAsync(reportId, request, adminId);
        return NoContent();
    
    }
}
