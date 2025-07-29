using Microsoft.Extensions.Logging;
using System.Text;
using System.Globalization;
using CsvHelper;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.DTOs;

namespace ClockifyData.Application.Services;

public class ExportService : IExportService
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ILogger<ExportService> _logger;

    public ExportService(
        ITimeEntryRepository timeEntryRepository,
        ILogger<ExportService> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<byte[]> ExportReportToCsvAsync(DateTime from, DateTime to)
    {
        _logger.LogInformation("Exporting time entries report from {From} to {To}", from, to);

        var timeEntries = await _timeEntryRepository.GetTimeEntriesByDateRangeAsync(from, to);
        
        _logger.LogInformation("Retrieved {Count} time entries from repository", timeEntries.Count());
        
        if (!timeEntries.Any())
        {
            _logger.LogWarning("No time entries found for the date range {From} to {To}", from, to);
            // Return empty CSV with headers
            return Encoding.UTF8.GetBytes("Date,User,Project,Task,Start Time,End Time,Duration (Hours)\r\n");
        }

        var csv = new StringBuilder();
        // Add CSV header
        csv.AppendLine("Date,User,Project,Task,Start Time,End Time,Duration (Hours)");

        foreach (var entry in timeEntries)
        {
            _logger.LogDebug("Processing entry: EntryId={EntryId}, UserId={UserId}, TaskId={TaskId}, Start={Start}, End={End}", 
                entry.EntryId, entry.UserId, entry.TaskId, entry.StartTime, entry.EndTime);
                
            var duration = (entry.EndTime - entry.StartTime).TotalHours;
            var line = $"{entry.StartTime:yyyy-MM-dd}," +
                      $"\"{entry.User?.FullName ?? "Unknown"}\"," +
                      $"\"{entry.Task?.Project?.Name ?? "Unknown"}\"," +
                      $"\"{entry.Task?.Name ?? "Unknown"}\"," +
                      $"{entry.StartTime:HH:mm}," +
                      $"{entry.EndTime:HH:mm}," +
                      $"{duration:F2}";
            
            csv.AppendLine(line);
        }

        var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
        
        _logger.LogInformation("Exported {Count} time entries to CSV ({Size} bytes)", 
            timeEntries.Count(), csvBytes.Length);

        return csvBytes;
    }

    public async System.Threading.Tasks.Task<byte[]> ExportCurrentMonthReportToCsvAsync()
    {
        var currentDate = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        _logger.LogInformation("Exporting current month report from {From} to {To}", firstDayOfMonth, lastDayOfMonth);

        var timeEntries = await _timeEntryRepository.GetTimeEntriesByDateRangeAsync(firstDayOfMonth, lastDayOfMonth);
        
        _logger.LogInformation("Retrieved {Count} time entries from repository", timeEntries.Count());
        
        if (!timeEntries.Any())
        {
            _logger.LogWarning("No time entries found for the current month {Month}/{Year}", currentDate.Month, currentDate.Year);
            // Return empty CSV with headers
            using var emptyMemoryStream = new MemoryStream();
            using var emptyWriter = new StreamWriter(emptyMemoryStream, Encoding.UTF8);
            using var emptyCsv = new CsvWriter(emptyWriter, CultureInfo.InvariantCulture);
            
            emptyCsv.WriteHeader<MonthlyReportExportDto>();
            await emptyWriter.FlushAsync();
            return emptyMemoryStream.ToArray();
        }

        // Transform to monthly report DTOs with null-safe handling
        var reportData = timeEntries.Select(entry => new MonthlyReportExportDto
        {
            UserFullName = entry.User?.FullName ?? "Unknown User",
            ProjectName = entry.Task?.Project?.Name ?? "Unknown Project",
            TaskName = entry.Task?.Name ?? "Unknown Task",
            StartTime = entry.StartTime,
            EndTime = entry.EndTime,
            Duration = FormatDuration(entry.EndTime - entry.StartTime)
        }).ToList();

        // Generate CSV using CsvHelper
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Configure CSV headers
        csv.WriteHeader<MonthlyReportExportDto>();
        await csv.NextRecordAsync();

        // Write data records
        foreach (var record in reportData)
        {
            csv.WriteRecord(record);
            await csv.NextRecordAsync();
        }

        await writer.FlushAsync();
        var csvBytes = memoryStream.ToArray();

        _logger.LogInformation("Exported {Count} current month time entries to CSV ({Size} bytes)", 
            reportData.Count, csvBytes.Length);

        return csvBytes;
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{duration.TotalHours:F2} hours";
        }
        else
        {
            return $"{duration.TotalMinutes:F0} minutes";
        }
    }
}
