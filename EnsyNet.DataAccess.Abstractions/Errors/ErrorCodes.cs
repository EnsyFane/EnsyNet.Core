namespace EnsyNet.DataAccess.Abstractions.Errors;

public static class ErrorCodes
{
    public const string ENTITY_NOT_FOUND_ERROR = "[EntityNotFoundError]";
    public const string UNEXPECTED_DATABASE_ERROR = "[UnexpectedDatabaseError]";
    public const string BULK_DELETE_OPERATION_FAILED_ERROR = "[BulkDeleteOperationFailedError]";
    public const string BULK_INSERT_OPERATION_FAILED_ERROR = "[BulkInsertOperationFailedError]";
    public const string BULK_UPDATE_OPERATION_FAILED_ERROR = "[BulkUpdateOperationFailedError]";
    public const string DELETE_OPERATION_FAILED_ERROR = "[DeleteOperationFailedError]";
    public const string INSERT_OPERATION_FAILED_ERROR = "[InsertOperationFailedError]";
    public const string UPDATE_OPERATION_FAILED_ERROR = "[UpdateOperationFailedError]";
}
