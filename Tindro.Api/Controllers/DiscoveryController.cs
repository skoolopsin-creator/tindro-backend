using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Location.Dtos;
using Tindro.Application.Location.Services;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers;

[ApiController]
[Route("api/v1/discovery")]
[Authorize]
public class DiscoveryController : ControllerBase
{
    private readonly CrossedPathsService _crossedPathsService;
    private readonly MapCardService _mapCardService;

    public DiscoveryController(
        CrossedPathsService crossedPathsService,
        MapCardService mapCardService)
    {
        _crossedPathsService = crossedPathsService;
        _mapCardService = mapCardService;
    }

    /// <summary>
    /// Get users you've crossed paths with
    /// TTL: 7 days
    /// </summary>
    [HttpGet("crossed-paths")]
    public async Task<ActionResult<CrossedPathsResponse>> GetCrossedPaths(
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();

        var response = await _crossedPathsService.GetCrossedPathsAsync(userId);
        return Ok(response);
    }

    /// <summary>
    /// Get map card with UI-safe zone positions
    /// No GPS coordinates returned
    /// Verified-only option available
    /// </summary>
    [HttpGet("map-card")]
    public async Task<ActionResult<MapCardResponse>> GetMapCard(
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
     

        var response = await _mapCardService.GetMapCardAsync(userId);
        return Ok(response);
    }

    /// <summary>
    /// Update location privacy settings
    /// </summary>
    [HttpPost("privacy-settings")]
    public async Task<ActionResult<LocationPrivacySettingsResponse>> UpdatePrivacySettings(
        [FromBody] LocationPrivacySettingsRequest request,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        

        // TODO: Implement privacy settings update
        return Ok(new LocationPrivacySettingsResponse
        {
            IsLocationEnabled = request.IsLocationEnabled,
            HideDistance = request.HideDistance,
            IsPaused = request.IsPaused,
            VerifiedOnlyMap = request.VerifiedOnlyMap
        });
    }

    /// <summary>
    /// Get current privacy settings
    /// </summary>
    [HttpGet("privacy-settings")]
    public async Task<ActionResult<LocationPrivacySettingsResponse>> GetPrivacySettings(
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
       

        // TODO: Implement privacy settings retrieval
        return Ok(new LocationPrivacySettingsResponse
        {
            IsLocationEnabled = false,
            HideDistance = false,
            IsPaused = false,
            VerifiedOnlyMap = false
        });
    }
}
