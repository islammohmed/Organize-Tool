using ClockifyData.Domain.Entities;

namespace ClockifyData.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByFullNameAsync(string fullName, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersWithProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersWithTimeEntriesAsync(CancellationToken cancellationToken = default);
}
