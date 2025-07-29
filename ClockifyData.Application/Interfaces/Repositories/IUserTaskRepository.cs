using ClockifyData.Domain.Entities;

namespace ClockifyData.Application.Interfaces.Repositories;

public interface IUserTaskRepository : IGenericRepository<UserTask>
{
    Task<IEnumerable<UserTask>> GetTasksAssignedToUserAsync(int userId);
    Task<IEnumerable<UserTask>> GetUsersAssignedToTaskAsync(int taskId);
    Task<UserTask?> GetAssignmentAsync(int userId, int taskId);
}
