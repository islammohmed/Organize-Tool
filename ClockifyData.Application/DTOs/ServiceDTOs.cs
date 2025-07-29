namespace ClockifyData.Application.DTOs;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string? ClockifyId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
}

public class TaskDto
{
    public int TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public decimal EstimateHours { get; set; }
    public string? ClockifyId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class CreateTaskDto
{
    public string Name { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public decimal EstimateHours { get; set; }
}

public class UserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
}

public class AssignTaskDto
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
}

public class TimeEntryDto
{
    public int EntryId { get; set; }
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? ClockifyId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public TimeSpan Duration => EndTime - StartTime;
}

public class CreateTimeEntryDto
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
