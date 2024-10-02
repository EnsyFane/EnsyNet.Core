using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.Cleanup.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EnsyNet.DataAccess.Cleanup.Implementations;

/// <summary>
/// Base implementation of the data access cleanup service.
/// </summary>
/// <typeparam name="T">Type of the entity to be cleaned up.</typeparam>
public class BaseDataAccessCleanupService<T> : IDataAccessCleanupService<T> where T : DbEntity
{
    private readonly IRepository<T> _repository;
    private readonly ILogger<BaseDataAccessCleanupService<T>> _logger;

    public BaseDataAccessCleanupService(
        IRepository<T> repository,
        ILogger<BaseDataAccessCleanupService<T>> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// This expression is used to filter entities that can be soft deleted. Should be overridden if filtering of soft deleted entities is required.
    /// </summary>
    protected virtual Expression<Func<T, bool>> FilterEntitiesToHardDelete { get; set; } = x => true;

    /// <summary>
    /// This method is called before the deletion of an entity. Should be overridden to add custom logic to be executed before deletion.
    /// </summary>
    /// <param name="entity">The entity that will be deleted at the end of the <see cref="CleanDataAccess(DateTime, int, CancellationToken)"/> method has been executed.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// <see cref="Result.Ok()"/> if the operation was successful and the entity can be deleted or <br/>
    /// A <see cref="Result"/> with an error if the operation failed and the entity should not be deleted."/>
    /// </returns>
    protected virtual Task<Result> ExecuteBeforeDeletion(T entity, CancellationToken ct) 
        => Task.FromResult(Result.Ok());

    /// <inheritdoc />
    public virtual async Task<Result<int>> CleanDataAccess(DateTime cutoffDate, int batchCount, CancellationToken ct)
    {
        var paginationQuery = new PaginationQuery()
        {
            Take = batchCount,
            Skip = 0,
        };
        var entitiesToDeleteResult = await _repository
            .GetSoftDeleted(FilterEntitiesToHardDelete, cutoffDate, paginationQuery, ct);
        if (entitiesToDeleteResult.HasError)
        {
            _logger.LogError("An error occurred while trying to get the entities to delete. Error: {Error}", entitiesToDeleteResult.Error);
            return Result.FromError<int>(entitiesToDeleteResult.Error!);
        }
        var entitiesToDelete = entitiesToDeleteResult.Data!;

        if (!entitiesToDelete.Any())
        {
            _logger.LogInformation("No soft deleted {EntityType}s to clean.", typeof(T).Name);
            return Result.Ok(0);
        }

        var entityToShouldDeleteMapTasks = entitiesToDelete
            .Select(async entity =>
                {
                    var beforeDeletionResult = await ExecuteBeforeDeletion(entity, ct);
                    if (beforeDeletionResult.HasError)
                    {
                        _logger.LogWarning("{EntityType} with Id {EntityId} will not be hard deleted because an error occurred while executing the before deletion method. Error: {Error}", typeof(T).Name, entity.Id, beforeDeletionResult.Error);
                        return new
                        {
                            Id = entity.Id,
                            ShouldDeleted = false,
                        };
                    }

                    return new
                    {
                        Id = entity.Id,
                        ShouldDeleted = true,
                    };
                });

        var entitiesThatCanBeDeleted = (await Task.WhenAll(entityToShouldDeleteMapTasks.ToArray()))
            .Where(x => x.ShouldDeleted)
            .Select(x => x.Id)
            .ToList();

        var hardDeleteResult = await _repository.HardDelete(entitiesThatCanBeDeleted, ct);

        return hardDeleteResult;
    }
}
