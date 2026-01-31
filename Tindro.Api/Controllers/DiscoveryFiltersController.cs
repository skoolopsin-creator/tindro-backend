using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Application.Discovery.Interfaces;
using Tindro.Application.Discovery.Dtos;

namespace Tindro.Api.Controllers;

/// <summary>
/// Advanced discovery filters controller
/// </summary>
[ApiController]
[Route("api/v1/discovery/filters")]
[Authorize]
public class DiscoveryFiltersController : ControllerBase
{
    private readonly IFilterService _filterService;

    public DiscoveryFiltersController(IFilterService filterService)
    {
        _filterService = filterService;
    }

    /// <summary>
    /// Apply discovery filters and get matching profiles
    /// </summary>
    [HttpPost("apply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApplyFilter([FromBody] ApplyFilterRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var result = await _filterService.ApplyFilterAsync(userId, request);
        return Ok(result);
    }

    /// <summary>
    /// Validate filter criteria before applying
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateFilter([FromBody] FilterPreferencesDto preferences)
    {
        var validation = await _filterService.ValidateFilterAsync(preferences);
        return Ok(validation);
    }

    /// <summary>
    /// Save user filter preferences
    /// </summary>
    [HttpPost("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveFilterPreferences([FromBody] FilterPreferencesDto preferencesDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var saved = await _filterService.SaveFilterPreferencesAsync(userId, preferencesDto);
        return Ok(saved);
    }

    /// <summary>
    /// Get user filter preferences
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterPreferences([FromQuery] string? filterName = null)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var preferences = await _filterService.GetFilterPreferencesAsync(userId, filterName);
        return Ok(preferences);
    }

    /// <summary>
    /// Get all user filter preferences
    /// </summary>
    [HttpGet("preferences/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFilterPreferences()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var filters = await _filterService.GetAllUserFiltersAsync(userId);
        return Ok(filters);
    }

    /// <summary>
    /// Delete filter preferences
    /// </summary>
    [HttpDelete("preferences/{filterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFilterPreferences(Guid filterId)
    {
        await _filterService.DeleteFilterAsync(filterId);
        return NoContent();
    }

    /// <summary>
    /// Create saved filter template
    /// </summary>
    [HttpPost("saved")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSavedFilter([FromBody] SavedFilterDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var saved = await _filterService.CreateSavedFilterAsync(userId, request.Name, request.FilterPreferences ?? new());
        return CreatedAtAction(nameof(GetSavedFilter), new { savedFilterId = saved.Id }, saved);
    }

    /// <summary>
    /// Get user saved filters
    /// </summary>
    [HttpGet("saved")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavedFilters()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var filters = await _filterService.GetSavedFiltersAsync(userId);
        return Ok(filters);
    }

    /// <summary>
    /// Get specific saved filter
    /// </summary>
    [HttpGet("saved/{savedFilterId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSavedFilter(Guid savedFilterId)
    {
        var filter = await _filterService.GetSavedFilterAsync(savedFilterId);
        return Ok(filter);
    }

    /// <summary>
    /// Delete saved filter
    /// </summary>
    [HttpDelete("saved/{savedFilterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedFilter(Guid savedFilterId)
    {
        await _filterService.DeleteSavedFilterAsync(savedFilterId);
        return NoContent();
    }

    /// <summary>
    /// Set default saved filter
    /// </summary>
    [HttpPut("saved/{savedFilterId}/default")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultFilter(Guid savedFilterId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var filter = await _filterService.SetDefaultFilterAsync(userId, savedFilterId);
        return Ok(filter);
    }

    /// <summary>
    /// Get filter usage analytics
    /// </summary>
    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterAnalytics()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var analytics = await _filterService.GetFilterAnalyticsAsync(userId);
        return Ok(analytics);
    }

    /// <summary>
    /// Estimate filter results count
    /// </summary>
    [HttpPost("estimate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EstimateFilterResults([FromBody] FilterPreferencesDto preferences)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var count = await _filterService.EstimateFilterResultsAsync(userId, preferences);
        return Ok(new { estimatedCount = count });
    }
}
