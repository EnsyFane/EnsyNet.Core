using Microsoft.EntityFrameworkCore;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

internal static class DatabaseConfiguration
{
    public static DbContextOptions<TestDbContext> Options { get; }

    static DatabaseConfiguration()
    {
        var connectionString = Environment.GetEnvironmentVariable("ENSY_NET_TEST_DB_CONNECTION_STRING") ?? "Server=localhost;Database=EnsyNetTests;User Id=sa;Password=Password1!;TrustServerCertificate=true";
        Options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        using var context = new TestDbContext(Options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}
