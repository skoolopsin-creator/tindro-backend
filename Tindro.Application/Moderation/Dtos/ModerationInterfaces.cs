using Tindro.Application.Moderation.Dtos;
using Tindro.Domain.Moderation;

namespace Tindro.Application.Moderation.Interfaces;

/// <summary>
/// Service for handling user reports and moderation actions.
/// </summary>
public interface IModerationService
{
    /// <summary>
    /// Creates a new report against a user.
    /// </summary>
    Task<ReportDto> CreateReportAsync(CreateReportRequestDto request, Guid reporterId);

    /// <summary>
    /// Retrieves a paginated list of reports based on query parameters.
    /// </summary>
    Task<PagedResult<ReportDto>> GetReportsAsync(ReportQueryParameters query);

    /// <summary>
    /// Retrieves a single report by its ID.
    /// </summary>
    Task<ReportDto> GetReportByIdAsync(Guid reportId);

    /// <summary>
    /// Resolves a report with a given action and notes.
    /// </summary>
    Task ResolveReportAsync(Guid reportId, ResolveReportRequestDto request, Guid adminId);
}

/// <summary>
/// Repository for moderation-related entities.
/// </summary>
public interface IModerationRepository
{
    /// <summary>
    /// Creates a new report in the database.
    /// </summary>
    Task<Report> CreateReportAsync(Report report);

    /// <summary>
    /// Retrieves a report by its ID.
    /// </summary>
    Task<Report?> GetReportByIdAsync(Guid reportId);

    /// <summary>
    /// Retrieves a list of reports based on query parameters.
    /// </summary>
    Task<(List<Report> reports, int totalCount)> GetReportsAsync(ReportQueryParameters query);

    /// <summary>
    /// Updates an existing report.
    /// </summary>
    Task<Report> UpdateReportAsync(Report report);
}