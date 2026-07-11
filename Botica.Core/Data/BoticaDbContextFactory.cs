using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Botica.Core.Data;

public class BoticaDbContextFactory : IDesignTimeDbContextFactory<BoticaDbContext>
{
    public BoticaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BoticaDbContext>();
        optionsBuilder.UseSqlite(BoticaDbPath.GetDefaultConnectionString());

        return new BoticaDbContext(optionsBuilder.Options);
    }
}
