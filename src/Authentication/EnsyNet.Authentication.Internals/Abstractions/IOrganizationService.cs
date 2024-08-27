using EnsyNet.Authentication.Core.Models;
using EnsyNet.Core.Results;

namespace EnsyNet.Authentication.Internals.Abstractions;

public interface IOrganizationService
{
    Task<Result<Organization>> GetOrganizationById(Guid id, CancellationToken ct);

    Task<Result<Organization>> GetOrganizationByName(string name, CancellationToken ct);

    Task<Result<Organization>> AddOrganization(Organization organization, CancellationToken ct);
    Task<Result> UpdateOrganizationName(Guid id, string name, CancellationToken ct);
    Task<Result> DeleteOrganization(Guid id, CancellationToken ct);
}
