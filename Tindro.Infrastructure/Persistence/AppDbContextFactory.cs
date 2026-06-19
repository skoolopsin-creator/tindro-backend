using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tindro.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use localhost for local migrations (Docker container accessible on localhost:5432)
        var connectionString = "Server=aws-1-ap-northeast-2.pooler.supabase.com;Port=5432;Database=postgres;User Id=postgres.ingpnnxrhbcmaqlnlbjv;Password=gkraGlCLYQhrziPMKvOGvSaNvxYnEGAK;";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
