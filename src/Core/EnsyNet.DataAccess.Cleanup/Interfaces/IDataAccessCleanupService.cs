using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Cleanup.Interfaces;

/// <summary>
/// Data access cleanup service interface.
/// </summary>
/// <typeparam name="T">Type of the entity to be cleaned up.</typeparam>
public interface IDataAccessCleanupService<T> where T : DbEntity
{
    /// <summary>
    /// Deletes the data access records that are older than the specified cutoff date.
    /// </summary>
    /// <remarks>Only deletes data that has been soft deleted. Data is considered as soft deleted if <see cref="DbEntity.DeletedAt"/> is not null.</remarks>
    /// <param name="cutoffDate">Cutoff date for deleting data. If <see cref="DbEntity.DeletedAt"/> is before this date then the data will be soft deleted.</param>
    /// <param name="batchCount">The maximum amount of records to delete when this method is called.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of records that were deleted or <br/>
    /// An <see cref="UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> CleanDataAccess(DateTime cutoffDate, int batchCount, CancellationToken ct);
}
