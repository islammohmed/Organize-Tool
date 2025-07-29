using ClockifyData.Domain.Entities;
using DomainTask = ClockifyData.Domain.Entities.Task;

namespace ClockifyData.Application.Interfaces.Repositories;

public interface ITaskRepository : IGenericRepository<DomainTask>
{
    Task<IEnumerable<DomainTask>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainTask>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<DomainTask?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainTask>> GetTasksWithTimeEntriesAsync(CancellationToken cancellationToken = default);
    Task<DomainTask?> GetWithTimeEntriesByIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainTask>> GetByEstimateHoursGreaterThanAsync(decimal minHours, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainTask>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainTask>> GetUnsyncedTasksAsync(CancellationToken cancellationToken = default);
}
