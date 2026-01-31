using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Domain.Stories;

namespace Tindro.Application.Common.Interfaces
{
    public interface IStoriesRepository
    {
        Task<List<Story>> GetExpiredStoriesAsync(DateTime now, CancellationToken ct = default);
        Task DeleteAsync(Guid storyId, CancellationToken ct = default);
    }
}
