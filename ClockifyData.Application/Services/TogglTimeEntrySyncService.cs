using Microsoft.Extensions.Logging;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.DTOs;

namespace ClockifyData.Application.Services;

public class TogglTimeEntrySyncService : ITimeEntrySyncService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TogglTimeEntrySyncService> _logger;
    
    public string ProviderName => "Toggl";

    public TogglTimeEntrySyncService(
        IHttpClientFactory httpClientFactory,
        ILogger<TogglTimeEntrySyncService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SyncAsync(List<TimeEntryDto> entries)
    {
        _logger.LogInformation("Starting sync of {Count} time entries to Toggl", entries.Count);

        // Simulate sync process (replace with actual Toggl API implementation)
        foreach (var entry in entries)
        {
            await SyncSingleEntryAsync(entry);
        }

        _logger.LogInformation("Completed Toggl sync for {Count} entries", entries.Count);
    }

    private async Task SyncSingleEntryAsync(TimeEntryDto entry)
    {
        // Simulate API call delay
        await Task.Delay(100);
        
        _logger.LogDebug("Simulated sync of time entry {EntryId} to Toggl", entry.EntryId);
        
        // TODO: Implement actual Toggl API integration
        // This is a placeholder implementation
    }
}
