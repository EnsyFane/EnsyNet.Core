namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error codes used in the data access layer.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// Error code for when an entity is not found.
    /// </summary>
    public const string EntityNotFoundError = "[EntityNotFoundError]";

    /// <summary>
    /// Error code for when the repository ran into an unexpected database error.
    /// </summary>
    public const string UnexpectedDatabaseError = "[UnexpectedDatabaseError]";

    /// <summary>
    /// Error code for when a bulk delete operation fails.
    /// </summary>
    public const string BulkDeleteOperationFailedError = "[BulkDeleteOperationFailedError]";

    /// <summary>
    /// Error code for when a delete operation fails.
    /// </summary>
    public const string DeleteOperationFailedError = "[DeleteOperationFailedError]";

    /// <summary>
    /// Error code for when a bulk insert operation fails.
    /// </summary>
    public const string BulkInsertOperationFailedError = "[BulkInsertOperationFailedError]";

    /// <summary>
    /// Error code for when an insert operation fails.
    /// </summary>
    public const string InsertOperationFailedError = "[InsertOperationFailedError]";

    /// <summary>
    /// Error code for when a bulk update operation fails.
    /// </summary>
    public const string BulkUpdateOperationFailedError = "[BulkUpdateOperationFailedError]";

    /// <summary>
    /// Error code for when an update operation fails.
    /// </summary>
    public const string UpdateOperationFailedError = "[UpdateOperationFailedError]";

    /// <summary>
    /// Error code for when an update operation fails due to an invalid expression provided by the user.
    /// </summary>
    public const string InvalidUpdateEntityExpressionError = "[InvalidUpdateEntityExpressionError]";
}
