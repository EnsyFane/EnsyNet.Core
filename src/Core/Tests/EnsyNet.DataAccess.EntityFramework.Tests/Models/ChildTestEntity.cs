using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Models;

public sealed record ChildTestEntity : DbEntity
{
    public required Guid ParentId { get; init; }
}
