using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Match
{
    public class MatchDto
    {
        public string MatchId { get; set; } = null!;
        public string MatchedWithUserId { get; set; } = null!;
        public string MatchedWithName { get; set; } = null!;
        public string? MatchedWithMainPhotoUrl { get; set; }
        public DateTime MatchedAt { get; set; }
        public bool IsNew { get; set; } // for unread badge etc
    }
}
