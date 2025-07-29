using Microsoft.EntityFrameworkCore;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Infrastructure.Data;
using DomainTask = ClockifyData.Domain.Entities.Task;

namespace ClockifyData.Infrastructure.Repositories.Implementations;

public class TaskRepository : GenericRepository<DomainTask>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DomainTask>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Project)
            .Include(t => t.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DomainTask>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Project)
            .Include(t => t.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainTask?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<DomainTask>> GetTasksWithTimeEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.TimeEntries)
            .Include(t => t.Project)
            .Include(t => t.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainTask?> GetWithTimeEntriesByIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.TimeEntries)
            .Include(t => t.Project)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TaskId == taskId, cancellationToken);
    }

    public async Task<IEnumerable<DomainTask>> GetByEstimateHoursGreaterThanAsync(decimal minHours, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.EstimateHours > minHours)
            .Include(t => t.Project)
            .Include(t => t.User)
            .ToListAsync(cancellationToken);
    }

    public override async Task<DomainTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.User)
            .Include(t => t.TimeEntries)
            .FirstOrDefaultAsync(t => t.TaskId == id, cancellationToken);
    }

    public async Task<IEnumerable<DomainTask>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.User)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DomainTask>> GetUnsyncedTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.User)
            .Where(t => string.IsNullOrEmpty(t.ClockifyId))
            .ToListAsync(cancellationToken);
    }
}
