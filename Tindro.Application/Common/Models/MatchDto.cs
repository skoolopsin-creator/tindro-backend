using System;

namespace Tindro.Application.Common.Models
{
    public class MatchDto
    {
        public string MatchId { get; set; } = null!;
        public string MatchedWithUserId { get; set; } = null!;
        public string MatchedWithName { get; set; } = null!;
        public string? MatchedWithMainPhotoUrl { get; set; }
        public DateTime MatchedAt { get; set; }
        public bool IsNew { get; set; }
    }
}
