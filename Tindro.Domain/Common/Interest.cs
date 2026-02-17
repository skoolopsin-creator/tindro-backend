using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Domain.Recommendations;

namespace Tindro.Domain.Common
{
    public class Interest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string IconKey { get; set; } = default!;

        public ICollection<UserInterest> UserInterests { get; set; }
    }

}
