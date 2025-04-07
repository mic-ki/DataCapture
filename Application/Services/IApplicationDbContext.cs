using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application.Services;


public interface IApplicationDbContext
{
    DbSet<T> Set<T>() where T : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public DatabaseFacade Database { get; }
}
