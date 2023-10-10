using Microsoft.EntityFrameworkCore;
using MiniApi.Model;

namespace MiniApi.Persistence.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Staff> Staffs => Set<Staff>();
    public DbSet<CurrentPrice> CurrentPrices => Set<CurrentPrice>();
}
