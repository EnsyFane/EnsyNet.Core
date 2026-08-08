using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ENSY_NET_TEST_DB_CONNECTION_STRING") ?? "Server=localhost;Database=EnsyNetTests;User Id=sa;Password=Password1!;TrustServerCertificate=true";
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new TestDbContext(options);
    }
}
