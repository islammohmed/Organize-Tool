using ClockifyData.Domain.Entities;

namespace ClockifyData.Application.Interfaces.Repositories;

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsWithTasksAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetWithTasksByIdAsync(int projectId, CancellationToken cancellationToken = default);
}
