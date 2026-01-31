using Microsoft.EntityFrameworkCore;

namespace Tindro.Infrastructure.Persistence
{
    public sealed class QueryDbContext : AppDbContext
    {
        public QueryDbContext(
            DbContextOptions<QueryDbContext> options
        ) : base(options)
        {
        }
    }
}
