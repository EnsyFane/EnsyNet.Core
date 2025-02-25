using EnsyNet.DataAccess.Abstractions.Attributes;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Models;

[DbEntityPartition(nameof(OrgId), 0)]
public sealed record OrgPartitionedEntity : DbEntity
{
    public string Name { get; init; } = string.Empty;
    public Guid OrgId { get; init; }
}
