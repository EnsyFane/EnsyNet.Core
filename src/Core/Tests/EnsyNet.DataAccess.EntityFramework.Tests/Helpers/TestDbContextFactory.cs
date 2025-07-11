﻿using Microsoft.EntityFrameworkCore.Design;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
        => new(DatabaseConfiguration.Options);
}
