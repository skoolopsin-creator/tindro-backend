using Microsoft.EntityFrameworkCore;

namespace Tindro.Infrastructure.Persistence
{
    public sealed class CommandDbContext : AppDbContext
    {
        public CommandDbContext(
            DbContextOptions<CommandDbContext> options
        ) : base(options)
        {
        }
    }
}
