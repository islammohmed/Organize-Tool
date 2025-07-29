namespace ClockifyData.Application.DTOs;

public class MonthlyReportExportDto
{
    public string UserFullName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Duration { get; set; } = string.Empty;
}
