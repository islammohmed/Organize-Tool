using ClockifyData.Application.DTOs;

namespace ClockifyData.Application.Interfaces.Services;

public interface IProjectService
{
    System.Threading.Tasks.Task<ProjectDto> AddProjectAsync(CreateProjectDto dto);
    System.Threading.Tasks.Task<ProjectDto?> GetProjectByIdAsync(int projectId);
    System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
}

public interface ITaskService
{
    System.Threading.Tasks.Task<TaskDto> AddTaskToProjectAsync(CreateTaskDto dto);
    System.Threading.Tasks.Task<TaskDto?> GetTaskByIdAsync(int taskId);
    System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId);
}

public interface IUserService
{
    System.Threading.Tasks.Task<UserDto> AddUserAsync(CreateUserDto dto);
    System.Threading.Tasks.Task<UserDto?> GetUserByIdAsync(int userId);
    System.Threading.Tasks.Task<IEnumerable<UserDto>> GetAllUsersAsync();
}

public interface IAssignmentService
{
    System.Threading.Tasks.Task AssignTaskToUserAsync(AssignTaskDto dto);
    System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetTasksAssignedToUserAsync(int userId);
    System.Threading.Tasks.Task<IEnumerable<UserDto>> GetUsersAssignedToTaskAsync(int taskId);
}

public interface ITimeEntryService
{
    System.Threading.Tasks.Task<TimeEntryDto> AddTimeEntryAsync(CreateTimeEntryDto dto);
    System.Threading.Tasks.Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByUserIdAsync(int userId);
    System.Threading.Tasks.Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByDateRangeAsync(DateTime from, DateTime to);
}

public interface IClockifySyncService
{
    System.Threading.Tasks.Task SyncProjectsAsync();
    System.Threading.Tasks.Task SyncTasksAsync();
    System.Threading.Tasks.Task SyncTimeEntriesAsync();
}

public interface IExportService
{
    System.Threading.Tasks.Task<byte[]> ExportReportToCsvAsync(DateTime from, DateTime to);
    System.Threading.Tasks.Task<byte[]> ExportCurrentMonthReportToCsvAsync();
}
