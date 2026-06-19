using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tindro.Infrastructure.Persistence;

public class CommandDbContextFactory
    : IDesignTimeDbContextFactory<CommandDbContext>
{
    public CommandDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<CommandDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=aws-1-ap-northeast-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ingpnnxrhbcmaqlnlbjv;Password=gkraGlCLYQhrziPMKvOGvSaNvxYnEGAK");

        return new CommandDbContext(optionsBuilder.Options);
    }
}