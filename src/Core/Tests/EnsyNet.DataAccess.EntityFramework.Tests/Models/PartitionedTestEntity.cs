using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Models;

public sealed record PartitionedTestEntity : PartitionedDbEntity
{
    public required string Name { get; init; }
    public required int Value { get; init; }
}
