using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Domain.Moderation
{
    public class ModerationResult
    {
        public bool IsSafe { get; set; }
        public string Reason { get; set; } = "";
    }

}
