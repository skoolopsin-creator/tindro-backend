using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Location.Dtos;
using Tindro.Application.Location.Services;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers;

[ApiController]
[Route("api/v1/location")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly LocationService _locationService;

    public LocationController(LocationService locationService)
    {
        _locationService = locationService;
    }

    /// <summary>
    /// Update user location with privacy measures
    /// Rate limited: max once per 30 minutes
    /// </summary>
    [HttpPost("update")]
    public async Task<ActionResult<LocationUpdateResponse>> UpdateLocation(
        [FromBody] LocationUpdateRequest request,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.Latitude < -90 || request.Latitude > 90)
            return BadRequest("Invalid latitude");

        if (request.Longitude < -180 || request.Longitude > 180)
            return BadRequest("Invalid longitude");

        var userId = User.GetUserId();
       

        var response = await _locationService.UpdateLocationAsync(userId, request);
        return Ok(response);
    }

    /// <summary>
    /// Get nearby users within specified radius
    /// Privacy filters applied automatically
    /// </summary>
    [HttpGet("nearby")]
    public async Task<ActionResult<NearbyUsersResponse>> GetNearbyUsers(
        [FromQuery] double radius = 5,
        [FromQuery] int? ageMin = null,
        [FromQuery] int? ageMax = null,
        [FromQuery] string? gender = null,
        CancellationToken ct = default)
    {
        if (radius < 0.5 || radius > 50)
            return BadRequest("Radius must be between 0.5 and 50 km");

        var userId = User.GetUserId();
     

        var response = await _locationService.GetNearbyUsersAsync(
            userId,
            radius,
            ageMin,
            ageMax,
            gender
        );

        return Ok(response);
    }
}
