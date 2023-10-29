using Microsoft.EntityFrameworkCore;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Test;

[TestClass]
public class Initialize
{
    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(MockDataUtility.SQLiteConnectionString)
            .Options;
        await using var context = new ApplicationDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(MockDataUtility.SQLiteConnectionString)
            .Options;
        await using var context = new ApplicationDbContext(contextOptions);
        
        await context.Database.EnsureDeletedAsync();
    }
}