using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Models;

[PublicAPI]
public abstract record PartitionedDbEntity : DbEntity
{
    public Guid OrgId { get; init; }
}
