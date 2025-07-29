using Microsoft.Extensions.Logging;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Mappings;

namespace ClockifyData.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TaskDto> AddTaskToProjectAsync(CreateTaskDto dto)
    {
        _logger.LogInformation("Creating new task: {TaskName} for project: {ProjectId}", dto.Name, dto.ProjectId);

        // Start transaction for data consistency
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate project exists
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
            if (project == null)
            {
                throw new ArgumentException($"Project with ID {dto.ProjectId} not found");
            }

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {dto.UserId} not found");
            }

            var task = dto.ToEntity();
            var savedTask = await _taskRepository.AddAsync(task);
            
            // Save changes to database
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            // Load navigation properties for DTO mapping
            savedTask.Project = project;
            savedTask.User = user;

            _logger.LogInformation("Task created successfully with ID: {TaskId}", savedTask.TaskId);
            
            return savedTask.ToDto();
        }
        catch
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        return task?.ToDto();
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId)
    {
        var tasks = await _taskRepository.GetTasksByProjectIdAsync(projectId);
        return tasks.Select(t => t.ToDto());
    }
}
