using Microsoft.EntityFrameworkCore;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Domain.Entities;
using ClockifyData.Infrastructure.Data;

namespace ClockifyData.Infrastructure.Repositories.Implementations;

public class TimeEntryRepository : GenericRepository<TimeEntry>, ITimeEntryRepository
{
    public TimeEntryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimeEntry>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.UserId == userId)
            .Include(te => te.User)
            .Include(te => te.Task)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.TaskId == taskId)
            .Include(te => te.User)
            .Include(te => te.Task)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        return await _dbSet
            .Where(te => te.StartTime >= startDate && te.EndTime <= endDate)
            .Include(te => te.User)
            .Include(te => te.Task)
            .OrderBy(te => te.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        return await _dbSet
            .Where(te => te.UserId == userId && te.StartTime >= startDate && te.EndTime <= endDate)
            .Include(te => te.User)
            .Include(te => te.Task)
            .OrderBy(te => te.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetTotalHoursByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var timeEntries = await _dbSet
            .Where(te => te.UserId == userId)
            .ToListAsync(cancellationToken);

        return timeEntries.Sum(te => (te.EndTime - te.StartTime).TotalHours);
    }

    public async Task<double> GetTotalHoursByTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var timeEntries = await _dbSet
            .Where(te => te.TaskId == taskId)
            .ToListAsync(cancellationToken);

        return timeEntries.Sum(te => (te.EndTime - te.StartTime).TotalHours);
    }

    public override async Task<TimeEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(te => te.User)
            .Include(te => te.Task)
                .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(te => te.EntryId == id, cancellationToken);
    }
}
