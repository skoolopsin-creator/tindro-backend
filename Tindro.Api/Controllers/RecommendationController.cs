using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Application.Recommendations.Interfaces;
using Tindro.Application.Recommendations.Dtos;
using Tindro.Domain.Recommendations;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers;

[ApiController]
[Route("api/v1/recommendations")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly IPreferenceRepository _preferenceRepo;
    private readonly ISkipRepository _skipRepo;
    private readonly IInterestRepository _interestrepo;

    public RecommendationController(
        IRecommendationService recommendationService,
        IPreferenceRepository preferenceRepo,
        ISkipRepository skipRepo,
        IInterestRepository interestRepository)
    {
        _recommendationService = recommendationService;
        _preferenceRepo = preferenceRepo;
        _skipRepo = skipRepo;
        _interestrepo = interestRepository;
    }

    /// <summary>
    /// Get personalized recommendations based on preferences
    /// </summary>
    [HttpGet("discover")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecommendations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "score")
    {
            var userId = User.GetUserId();

        var filter = new RecommendationFilterDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy
        };

        var recommendations = await _recommendationService.GetRecommendationsAsync(userId, filter);

        return Ok(new
        {
            recommendations = recommendations,
            totalCount = recommendations.Count(),
            page,
            pageSize,
            hasMore = recommendations.Count() == pageSize
        });
    }

    /// <summary>
    /// Get compatibility score between two profiles
    /// </summary>
    [HttpGet("score/{targetUserId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompatibilityScore(Guid targetUserId)
    {
        var userId = User.GetUserId();

        var score = await _recommendationService.CalculateCompatibilityAsync(userId, targetUserId);

        if (score == null)
            return NotFound();

        return Ok(score);
    }

    /// <summary>
    /// Get user's preference settings
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPreferences()
    {
        var userId = User.GetUserId();

        var preferences = await _preferenceRepo.GetOrCreatePreferencesAsync(userId);

        var dto = new UserPreferencesDto
        {
            MinAgePreference = preferences.MinAgePreference,
            MaxAgePreference = preferences.MaxAgePreference,
            MinHeightPreference = preferences.MinHeightPreference,
            MaxHeightPreference = preferences.MaxHeightPreference,
            MaxDistancePreference = preferences.MaxDistancePreference,
            SmokingPreference = preferences.SmokingPreference,
            DrinkingPreference = preferences.DrinkingPreference,
            WantChildrenPreference = preferences.WantChildrenPreference,
            HaveChildrenPreference = preferences.HaveChildrenPreference,
            EducationPreference = preferences.EducationPreference,
            RelationshipType = preferences.RelationshipType,
            ReligionPreference = preferences.ReligionPreference,
            EthnicityPreference = preferences.EthnicityPreference,
            InterestCategories = preferences.InterestCategories,
            PersonalityTraits = preferences.PersonalityTraits,
            OnlyVerifiedProfiles = preferences.OnlyVerifiedProfiles,
            HideInactiveProfiles = preferences.HideInactiveProfiles
        };

        return Ok(dto);
    }

    /// <summary>
    /// Update user's preference settings
    /// </summary>
    [HttpPost("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferencesDto dto)
    {
        var userId = User.GetUserId();

        var preferences = await _preferenceRepo.GetOrCreatePreferencesAsync(userId);

        preferences.MinAgePreference = dto.MinAgePreference;
        preferences.MaxAgePreference = dto.MaxAgePreference;
        preferences.MinHeightPreference = dto.MinHeightPreference;
        preferences.MaxHeightPreference = dto.MaxHeightPreference;
        preferences.MaxDistancePreference = dto.MaxDistancePreference;
        preferences.SmokingPreference = dto.SmokingPreference;
        preferences.DrinkingPreference = dto.DrinkingPreference;
        preferences.WantChildrenPreference = dto.WantChildrenPreference;
        preferences.HaveChildrenPreference = dto.HaveChildrenPreference;
        preferences.EducationPreference = dto.EducationPreference;
        preferences.RelationshipType = dto.RelationshipType;
        preferences.ReligionPreference = dto.ReligionPreference;
        preferences.EthnicityPreference = dto.EthnicityPreference;
        preferences.InterestCategories = dto.InterestCategories ?? new List<string>();
        preferences.PersonalityTraits = dto.PersonalityTraits ?? new List<string>();
        preferences.OnlyVerifiedProfiles = dto.OnlyVerifiedProfiles;
        preferences.HideInactiveProfiles = dto.HideInactiveProfiles;

        await _preferenceRepo.UpdatePreferencesAsync(preferences);

        return Ok(new { message = "Preferences updated successfully" });
    }

    /// <summary>
    /// Add an interest to user's profile
    /// </summary>
    [HttpPost("interests")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddInterest([FromBody] AddInterestRequest request)
    {
        var userId = User.GetUserId();

        var interest = await _interestrepo.GetByIdAsync(request.InterestId);
        if (interest == null)
            return BadRequest("Invalid interest");

        // 2?? Check duplicate
        var exists = await _preferenceRepo.ExistsAsync(userId, request.InterestId);
        if (exists)
            return BadRequest("Interest already added");



        var userInterest = new UserInterest
        {
            UserId = userId,
            InterestId = request.InterestId,
            ConfidenceScore = request.ConfidenceScore
        };

        await _preferenceRepo.AddInterestAsync(userInterest);

        return Ok(new
        {
            interest.Id,
            interest.Name,
            interest.Category
        });
    }

    /// <summary>
    /// Get user's interests
    /// </summary>
    [HttpGet("interests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInterests()
    {
        var userId = User.GetUserId();

        var interests = await _preferenceRepo.GetUserInterestsAsync(userId);

        var dtos = interests.Select(i => new UserInterestDto
        {
            InterestId = i.InterestId,
            InterestName = i.Interest.Name,
            Category = i.Interest.Category,
            IconKey = i.Interest.IconKey,
            ConfidenceScore = i.ConfidenceScore
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Skip/hide a profile from recommendations
    /// </summary>
    [HttpPost("skip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SkipProfile([FromBody] SkipProfileRequest request)
    {
        var userId = User.GetUserId();

        var existing = await _skipRepo.GetSkipAsync(userId, request.SkippedUserId);
        if (existing != null)
            return BadRequest("Profile already skipped");

        var skip = new SkipProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SkippedUserId = request.SkippedUserId,
            Reason = request.Reason,
            ExpiresAt = DateTime.UtcNow.AddDays(90)
        };

        await _skipRepo.CreateSkipAsync(skip);

        return Ok(new { message = "Profile skipped successfully" });
    }

    /// <summary>
    /// Prefetch recommendations for better performance
    /// </summary>
    [HttpPost("prefetch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PrefetchRecommendations()
    {
        var userId = User.GetUserId();

        await _recommendationService.PrefetchRecommendationsAsync(userId);

        return Ok(new { message = "Recommendations prefetched" });
    }
}
