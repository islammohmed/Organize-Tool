namespace ClockifyData.Application.DTOs.Clockify;

public class ClockifyProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#03DAC6";
    public bool IsPublic { get; set; } = true;
    public bool Billable { get; set; } = false;
}

public class ClockifyProjectResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool Billable { get; set; }
    public string WorkspaceId { get; set; } = string.Empty;
}

public class ClockifyTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string? EstimateType { get; set; } = "MANUAL";
    public ClockifyEstimate? Estimate { get; set; }
}

public class ClockifyEstimate
{
    public string Estimate { get; set; } = string.Empty;
    public string Type { get; set; } = "HOUR";
}

public class ClockifyTaskResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public ClockifyEstimate? Estimate { get; set; }
}

public class ClockifyTimeEntryRequest
{
    public string Start { get; set; } = string.Empty;
    public string? End { get; set; }
    public string ProjectId { get; set; } = string.Empty;
    public string? TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ClockifyTimeEntryResponse
{
    public string Id { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public string? End { get; set; }
    public string ProjectId { get; set; } = string.Empty;
    public string? TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
