using Microsoft.EntityFrameworkCore;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Domain.Entities;
using ClockifyData.Infrastructure.Data;

namespace ClockifyData.Infrastructure.Repositories.Implementations;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsWithTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .Include(p => p.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetWithTasksByIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId, cancellationToken);
    }

    public override async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.ProjectId == id, cancellationToken);
    }
}
