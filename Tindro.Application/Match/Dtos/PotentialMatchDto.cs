using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Match
{
    public class PotentialMatchDto
    {
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string? MainPhotoUrl { get; set; }
        public string? Bio { get; set; }
        public double DistanceKm { get; set; }
        public string[]? Interests { get; set; }
        public DateTime LastActive { get; set; }
    }
}
