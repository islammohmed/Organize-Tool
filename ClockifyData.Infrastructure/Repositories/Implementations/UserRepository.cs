using Microsoft.EntityFrameworkCore;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Domain.Entities;
using ClockifyData.Infrastructure.Data;

namespace ClockifyData.Infrastructure.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByFullNameAsync(string fullName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        
        return await _dbSet
            .FirstOrDefaultAsync(u => u.FullName == fullName, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersWithProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Projects)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersWithTimeEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.TimeEntries)
            .ToListAsync(cancellationToken);
    }

    public override async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Temporarily removing includes to avoid schema mismatch issues
        // TODO: Apply migrations properly to include ClockifyId columns
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
    }
}
