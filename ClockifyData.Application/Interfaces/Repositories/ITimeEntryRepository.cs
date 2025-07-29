using ClockifyData.Domain.Entities;

namespace ClockifyData.Application.Interfaces.Repositories;

public interface ITimeEntryRepository : IGenericRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<double> GetTotalHoursByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<double> GetTotalHoursByTaskAsync(int taskId, CancellationToken cancellationToken = default);
}
