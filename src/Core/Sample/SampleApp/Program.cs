﻿using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

using System.Linq.Expressions;

_ = Result.Ok();
Result<string> r2 = Result.Ok("Hello, World!");

_ = Result.FromError(new BulkDeleteOperationFailedError());

Console.WriteLine(r2.Data);
var cts = new CancellationTokenSource();

var entity = new TestEntity();
var entities = new List<TestEntity> { entity };
var pagination = new PaginationQuery() { Skip = 0, Take = 100 };
var sorting = new SortingQuery<TestEntity>()
{
    IsAscending = true,
    SortFieldSelector = e => e.Id,
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
await repository.Update(entity.Id, x => x.AddUpdate(e => e.Name, _ => "NewName"), cts.Token);
await repository.Update(new Dictionary<Guid, Expression<Func<EntityUpdates<TestEntity>, EntityUpdates<TestEntity>>>>{ { entity.Id, x => x.AddUpdate(e => e.Name, _ => "NewName") } }, cts.Token);
await repository.SoftDelete(Guid.NewGuid(), cts.Token);
await repository.SoftDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.SoftDelete(e => e.UpdatedAt != null, cts.Token);
await repository.HardDelete(Guid.NewGuid(), cts.Token);
await repository.HardDelete(new List<Guid> { Guid.NewGuid() }, cts.Token);
await repository.HardDelete(e => e.UpdatedAt != null, cts.Token);

internal sealed record TestEntity : DbEntity
{
    public string Name { get; set; } = string.Empty;
}
