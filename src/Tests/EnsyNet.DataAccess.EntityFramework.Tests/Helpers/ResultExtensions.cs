using EnsyNet.Core.Results;

using FluentAssertions;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

internal static class ResultExtensions
{
    public static void AssertNoError(this Result result)
        => result.Should().BeEquivalentTo(Result.Ok());

    public static void AssertNoError<T>(this Result<T> result)
        => result.Should().BeEquivalentTo(Result<T>.Ok());
}
