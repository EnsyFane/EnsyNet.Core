using EnsyNet.Authentication.Services.Models;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnsyNet.Authentication.Internals.DataAccess;

internal sealed class OrganizationRepository : BaseRepository<OrganizationInternalModel>, IRepository<OrganizationInternalModel>
{
    public OrganizationRepository(DbContext dbContext, DbSet<OrganizationInternalModel> dbSet, ILogger<OrganizationRepository> logger) : base(dbContext, dbSet, logger)
    {
    }
}
