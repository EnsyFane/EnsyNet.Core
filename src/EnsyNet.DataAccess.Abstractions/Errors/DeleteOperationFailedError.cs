﻿using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a delete operation fails.
/// </summary>
public sealed record DeleteOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOperationFailedError"/> class.
    /// </summary>
    public DeleteOperationFailedError() : base(ErrorCodes.DELETE_OPERATION_FAILED_ERROR)
    {
    }
}
