﻿using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when an insert operation fails.
/// </summary>
[PublicAPI]
public sealed record InsertOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertOperationFailedError"/> class.
    /// </summary>
    public InsertOperationFailedError() : base(ErrorCodes.InsertOperationFailedError) { }
}
