// See https://aka.ms/new-console-template for more information
using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

var r = Result.Ok();
Result<string> r2 = Result.Ok("Hello, World!");

Console.WriteLine(r2.Data);
var cts = new CancellationTokenSource();

var entity = new TestEntity();
var entities = new List<TestEntity> { entity };
var filter = (TestEntity e) => e.UpdatedAt != null;
var pagination = new PaginationQuery() { Skip = 0, Take = 100 };
var sorting = new SortingQuery<TestEntity, Guid>()
{
    IsAscending = true,
    SortFieldSelector = e => e.Id
};

IRepository<TestEntity> repository = null!;
await repository.GetById(Guid.NewGuid(), cts.Token);
await repository.GetByExpression(filter, cts.Token);
await repository.GetAll(cts.Token);
await repository.GetAll(sorting, cts.Token);
await repository.GetMany(pagination, cts.Token);
await repository.GetMany(pagination, sorting, cts.Token);
await repository.GetManyByExpression(filter, cts.Token);
await repository.GetManyByExpression(filter, pagination, cts.Token);
await repository.GetManyByExpression(filter, sorting, cts.Token);
await repository.GetManyByExpression(filter, pagination, sorting, cts.Token);
await repository.Insert(entity, cts.Token);
await repository.Insert(entities, cts.Token);
await repository.Update(entity, cts.Token);
await repository.Update(entities, cts.Token);
await repository.SoftDelete(Guid.NewGuid(), cts.Token);
await repository.SoftDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.SoftDelete(filter, cts.Token);
await repository.HardDelete(Guid.NewGuid(), cts.Token);
await repository.HardDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.HardDelete(filter, cts.Token);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "Sample code. Not production.")]
public sealed record TestEntity : DbEntity { };
