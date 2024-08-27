using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.Authentication.Services.Models;

internal sealed record OrganizationInternalModel : DbEntity
{
    public required string Name { get; init; }
}
