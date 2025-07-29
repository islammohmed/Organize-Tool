using Microsoft.Extensions.Logging;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Mappings;
using ClockifyData.Domain.Entities;
using DomainTask = ClockifyData.Domain.Entities.Task;

namespace ClockifyData.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignmentService> logger)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task AssignTaskToUserAsync(AssignTaskDto dto)
    {
        _logger.LogInformation("Assigning task {TaskId} to user {UserId}", dto.TaskId, dto.UserId);

        // Start transaction for data consistency
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {dto.UserId} not found");
            }

            // Validate task exists
            var task = await _taskRepository.GetByIdAsync(dto.TaskId);
            if (task == null)
            {
                throw new ArgumentException($"Task with ID {dto.TaskId} not found");
            }

            // Check if task is already assigned to this user
            if (task.UserId == dto.UserId)
            {
                _logger.LogWarning("Task {TaskId} is already assigned to user {UserId}", dto.TaskId, dto.UserId);
                await _unitOfWork.CommitTransactionAsync(); // Commit even though no changes
                return;
            }

            // Assign task to user by updating the UserId
            task.UserId = dto.UserId;
            _taskRepository.Update(task);
            
            // Save changes to database
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();
            
            _logger.LogInformation("Task assignment updated successfully");
        }
        catch
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetTasksAssignedToUserAsync(int userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);
        return tasks.Select(t => t.ToDto());
    }

    public async System.Threading.Tasks.Task<IEnumerable<UserDto>> GetUsersAssignedToTaskAsync(int taskId)
    {
        // Since each task can only be assigned to one user in your schema
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task?.User != null)
        {
            return new[] { task.User.ToDto() };
        }
        return Enumerable.Empty<UserDto>();
    }
}
