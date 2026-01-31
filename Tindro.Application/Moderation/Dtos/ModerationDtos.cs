namespace Tindro.Application.Moderation.Dtos;

/// <summary>
/// A generic class for paginated results.
/// </summary>
/// <typeparam name="T">The type of items in the paged result.</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// DTO for creating a new user report.
/// </summary>
public class CreateReportRequestDto
{
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReportType { get; set; } = "profile"; // e.g., "profile", "message", "photo"
}

/// <summary>
/// DTO representing a user report for API responses.
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // "Pending", "Resolved", "Dismissed"
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
}

/// <summary>
/// Parameters for querying user reports.
/// </summary>
public class ReportQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; } // "Pending", "Resolved", "Dismissed"
    public Guid? TargetUserId { get; set; }
    public string SortBy { get; set; } = "ReportedAt";
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// DTO for resolving a user report.
/// </summary>
public class ResolveReportRequestDto
{
    public string ResolutionAction { get; set; } = string.Empty; // e.g., "BanUser", "WarnUser", "Dismiss"
    public string ResolutionNotes { get; set; } = string.Empty;
}