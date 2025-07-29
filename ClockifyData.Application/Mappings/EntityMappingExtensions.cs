using ClockifyData.Application.DTOs;
using ClockifyData.Domain.Entities;
using DomainTask = ClockifyData.Domain.Entities.Task;

namespace ClockifyData.Application.Mappings;

public static class EntityMappingExtensions
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            UserId = project.UserId,
            // ClockifyId = project.ClockifyId, // Commented out - not in database schema
            UserName = project.User?.FullName ?? string.Empty
        };
    }

    public static Project ToEntity(this CreateProjectDto dto, int? projectId = null)
    {
        return new Project
        {
            ProjectId = projectId ?? 0,
            Name = dto.Name,
            UserId = dto.UserId
        };
    }

    public static TaskDto ToDto(this DomainTask task)
    {
        return new TaskDto
        {
            TaskId = task.TaskId,
            Name = task.Name,
            ProjectId = task.ProjectId,
            UserId = task.UserId,
            EstimateHours = task.EstimateHours,
            // ClockifyId = task.ClockifyId, // Commented out - not in database schema
            ProjectName = task.Project?.Name ?? string.Empty,
            UserName = task.User?.FullName ?? string.Empty
        };
    }

    public static DomainTask ToEntity(this CreateTaskDto dto, int? taskId = null)
    {
        return new DomainTask
        {
            TaskId = taskId ?? 0,
            Name = dto.Name,
            ProjectId = dto.ProjectId,
            UserId = dto.UserId,
            EstimateHours = dto.EstimateHours
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            FullName = user.FullName
        };
    }

    public static User ToEntity(this CreateUserDto dto, int? userId = null)
    {
        return new User
        {
            UserId = userId ?? 0,
            FullName = dto.FullName
        };
    }

    public static TimeEntryDto ToDto(this TimeEntry timeEntry)
    {
        return new TimeEntryDto
        {
            EntryId = timeEntry.EntryId,
            UserId = timeEntry.UserId,
            TaskId = timeEntry.TaskId,
            StartTime = timeEntry.StartTime,
            EndTime = timeEntry.EndTime,
            // ClockifyId = timeEntry.ClockifyId, // Commented out - not in database schema
            UserName = timeEntry.User?.FullName ?? string.Empty,
            TaskName = timeEntry.Task?.Name ?? string.Empty,
            ProjectName = timeEntry.Task?.Project?.Name ?? string.Empty
        };
    }

    public static TimeEntry ToEntity(this CreateTimeEntryDto dto, int? entryId = null)
    {
        return new TimeEntry
        {
            EntryId = entryId ?? 0,
            UserId = dto.UserId,
            TaskId = dto.TaskId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };
    }
}
