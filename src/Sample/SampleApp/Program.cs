// See https://aka.ms/new-console-template for more information
using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore.Query;

var r = Result.Ok();
Result<string> r2 = Result.Ok("Hello, World!");

var rError = Result.FromError(new BulkDeleteOperationFailedError());

Console.WriteLine(r2.Data);
var cts = new CancellationTokenSource();

var entity = new TestEntity();
var entities = new List<TestEntity> { entity };
var pagination = new PaginationQuery() { Skip = 0, Take = 100 };
var sorting = new SortingQuery<TestEntity>()
{
    IsAscending = true,
    SortFieldSelector = e => e.Id
};

IRepository<TestEntity> repository = null!;
await repository.GetById(Guid.NewGuid(), cts.Token);
await repository.GetByExpression(e => e.UpdatedAt != null, cts.Token);
await repository.GetAll(cts.Token);
await repository.GetAll(sorting, cts.Token);
await repository.GetMany(pagination, cts.Token);
await repository.GetMany(pagination, sorting, cts.Token);
await repository.GetManyByExpression(e => e.UpdatedAt != null, cts.Token);
await repository.GetManyByExpression(e => e.UpdatedAt != null, pagination, cts.Token);
await repository.GetManyByExpression(e => e.UpdatedAt != null, sorting, cts.Token);
await repository.GetManyByExpression(e => e.UpdatedAt != null, pagination, sorting, cts.Token);
await repository.Insert(entity, cts.Token);
await repository.Insert(entities, cts.Token);
await repository.Update(entity.Id, x => x.SetProperty(e => e.Name, "NewName"), cts.Token);
await repository.Update(new Dictionary<Guid, Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>{ { entity.Id, x => x.SetProperty(e => e.Name, "NewName") } }, cts.Token);
await repository.SoftDelete(Guid.NewGuid(), cts.Token);
await repository.SoftDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.SoftDelete(e => e.UpdatedAt != null, cts.Token);
await repository.HardDelete(Guid.NewGuid(), cts.Token);
await repository.HardDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.HardDelete(e => e.UpdatedAt != null, cts.Token);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "Sample code. Not production.")]
public sealed record TestEntity : DbEntity
{
    public string Name { get; set; } = string.Empty;
}
