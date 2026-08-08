using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class DatabaseUnavailableTests
{
    [Fact]
    public async Task DatabaseUnreachable_GetById_ReturnsDatabaseUnavailableError()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer("Server=localhost,1;Database=EnsyNetTests;User Id=sa;Password=Password1!;TrustServerCertificate=true;Connect Timeout=2")
            .Options;
        using var dbContext = new TestDbContext(options);
        var repository = new TestRepository(dbContext);

        var result = await repository.GetById(Guid.NewGuid(), CancellationToken.None);

        result.HasError.Should().BeTrue();
        result.Error.Should().BeOfType<DatabaseUnavailableError>();
    }
}
