using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tindro.Application.Admin.Interfaces;

using Tindro.Application.Analytics.Dtos;
using Tindro.Application.Users.Dtos;
using Tindro.Application.Common.Models;

namespace Tindro.Api.Controllers
{
    /// <summary>
    /// Controller for administrative functions, providing a centralized dashboard for user management, analytics, and moderation.
    /// </summary>
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")] // This entire controller is restricted to users with the "Admin" role.
    public class AdminController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IAdminUserService _adminUserService;

        public AdminController(IAnalyticsService analyticsService, IAdminUserService adminUserService)
        {
            _analyticsService = analyticsService;
            _adminUserService = adminUserService;
        }

        /// <summary>
        /// Retrieves a summary of key metrics for the admin dashboard.
        /// </summary>
        [HttpGet("dashboard/summary")]
        [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = await _analyticsService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        /// <summary>
        /// Searches for users based on specified criteria.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(PagedResult<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUsers([FromQuery] UserSearchParameters query)
        {
            var users = await _adminUserService.SearchUsersAsync(query);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves detailed information for a specific user.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var userDetail = await _adminUserService.GetUserDetailAsync(userId);
            return Ok(userDetail);
        }

        // Further endpoints for user management (e.g., banning, unbanning, role assignment)
        // and detailed analytics reports would be added here.
    }
}