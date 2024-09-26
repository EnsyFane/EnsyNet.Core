namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error codes used in the data access layer.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// Error code for when an entity is not found.
    /// </summary>
    public const string ENTITY_NOT_FOUND_ERROR = "[EntityNotFoundError]";

    /// <summary>
    /// Error code for when the repository ran into an unexpected database error.
    /// </summary>
    public const string UNEXPECTED_DATABASE_ERROR = "[UnexpectedDatabaseError]";

    /// <summary>
    /// Error code for when a bulk delete operation fails.
    /// </summary>
    public const string BULK_DELETE_OPERATION_FAILED_ERROR = "[BulkDeleteOperationFailedError]";

    /// <summary>
    /// Error code for when a delete operation fails.
    /// </summary>
    public const string DELETE_OPERATION_FAILED_ERROR = "[DeleteOperationFailedError]";

    /// <summary>
    /// Error code for when a bulk insert operation fails.
    /// </summary>
    public const string BULK_INSERT_OPERATION_FAILED_ERROR = "[BulkInsertOperationFailedError]";

    /// <summary>
    /// Error code for when an insert operation fails.
    /// </summary>
    public const string INSERT_OPERATION_FAILED_ERROR = "[InsertOperationFailedError]";

    /// <summary>
    /// Error code for when a bulk update operation fails.
    /// </summary>
    public const string BULK_UPDATE_OPERATION_FAILED_ERROR = "[BulkUpdateOperationFailedError]";

    /// <summary>
    /// Error code for when an update operation fails.
    /// </summary>
    public const string UPDATE_OPERATION_FAILED_ERROR = "[UpdateOperationFailedError]";

    /// <summary>
    /// Error code for when an update operation fails due to an invalid expression provided by the user.
    /// </summary>
    public const string INVALID_UPDATE_ENTITY_EXPRESSION_ERROR = "[InvalidUpdateEntityExpressionError]";

    public const string NO_ENTITY_UPDATES_FOUND = "[NoEntityUpdatesFound]";
}
