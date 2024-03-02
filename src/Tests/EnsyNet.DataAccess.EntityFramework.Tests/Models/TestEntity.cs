using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Models;

public sealed record TestEntity : DbEntity
{
    public required string StringField { get; init; }
    public required int IntField { get; init; }
    public required bool BoolField { get; init; }
    public required float FloatField { get; init; }
    public required double DoubleField { get; init; }
    public required decimal DecimalField { get; init; }
    public required DateTime DateTimeField { get; init; }
    public required TimeSpan TimeSpanField { get; init; }
    public required Guid GuidField { get; init; }
}
