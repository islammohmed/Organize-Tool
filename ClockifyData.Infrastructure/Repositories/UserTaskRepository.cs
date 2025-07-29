using Microsoft.EntityFrameworkCore;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Domain.Entities;
using ClockifyData.Infrastructure.Data;
using ClockifyData.Infrastructure.Repositories.Implementations;

namespace ClockifyData.Infrastructure.Repositories;

public class UserTaskRepository : GenericRepository<UserTask>, IUserTaskRepository
{
    public UserTaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserTask>> GetTasksAssignedToUserAsync(int userId)
    {
        return await _context.Set<UserTask>()
            .Include(ut => ut.Task)
            .ThenInclude(t => t.Project)
            .Include(ut => ut.User)
            .Where(ut => ut.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserTask>> GetUsersAssignedToTaskAsync(int taskId)
    {
        return await _context.Set<UserTask>()
            .Include(ut => ut.User)
            .Include(ut => ut.Task)
            .ThenInclude(t => t.Project)
            .Where(ut => ut.TaskId == taskId)
            .ToListAsync();
    }

    public async Task<UserTask?> GetAssignmentAsync(int userId, int taskId)
    {
        return await _context.Set<UserTask>()
            .Include(ut => ut.User)
            .Include(ut => ut.Task)
            .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TaskId == taskId);
    }
}
