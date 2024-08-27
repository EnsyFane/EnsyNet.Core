using EnsyNet.Authentication.Core.Models;
using EnsyNet.Authentication.Internals.Abstractions;
using EnsyNet.Core.Results;

namespace EnsyNet.Authentication.Internals.Services;

internal sealed class OrganizationService : IOrganizationService
{
    public Task<Result<Organization>> AddOrganization(Organization organization, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteOrganization(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Organization>> GetOrganizationById(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Organization>> GetOrganizationByName(string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateOrganizationName(Guid id, string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
