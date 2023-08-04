// See https://aka.ms/new-console-template for more information
using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;
using System.Linq;

var r = Result.Ok();
Result<string> r2 = Result.Ok("Hello, World!");

Console.WriteLine(r2.Data);


var entity = new TestEntity();
var entities = new List<TestEntity> { entity };
var filter = (TestEntity e) => e.UpdatedAt != null;
var pagination = new PaginationQuery() { Skip = 0, Take = 100 };
var sorting = new SortingQuery<TestEntity, Guid>()
{
    Ascending = true,
    SortFieldSelector = e => e.Id
};

IRepository<TestEntity> repository = null!;
await repository.GetById(Guid.NewGuid());
await repository.GetOneByExpression(filter);
await repository.GetAll();
await repository.GetAll(sorting);
await repository.GetMany(pagination);
await repository.GetMany(pagination, sorting);
await repository.GetManyByExpression(filter);
await repository.GetManyByExpression(filter, pagination);
await repository.GetManyByExpression(filter, sorting);
await repository.GetManyByExpression(filter, pagination, sorting);
await repository.Insert(entity);
await repository.Insert(entities);
await repository.Update(entity);
await repository.Update(entities);
await repository.SoftDelete(Guid.NewGuid());
await repository.SoftDelete(new List<Guid> { Guid.NewGuid() });
await repository.SoftDelete(filter);
await repository.HardDelete(Guid.NewGuid());
await repository.HardDelete(new List<Guid> { Guid.NewGuid() });
await repository.HardDelete(filter);


public sealed record TestEntity : DbEntity { };