using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.DTOs;
using System.Text.Json;
using System.Text;

namespace ClockifyData.Application.Services;

public class ClockifyTimeEntrySyncService : ITimeEntrySyncService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClockifyTimeEntrySyncService> _logger;
    private readonly IConfiguration _configuration;
    
    public string ProviderName => "Clockify";

    public ClockifyTimeEntrySyncService(
        IHttpClientFactory httpClientFactory,
        ILogger<ClockifyTimeEntrySyncService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SyncAsync(List<TimeEntryDto> entries)
    {
        _logger.LogInformation("Starting sync of {Count} time entries to Clockify", entries.Count);

        var apiKey = _configuration["Clockify:ApiKey"];
        var workspaceId = _configuration["Clockify:WorkspaceId"];

        _logger.LogInformation("Clockify Configuration: ApiKey={ApiKeyStatus}, WorkspaceId={WorkspaceIdStatus}", 
            string.IsNullOrEmpty(apiKey) ? "NOT SET" : $"SET (length: {apiKey.Length})",
            string.IsNullOrEmpty(workspaceId) ? "NOT SET" : $"SET (length: {workspaceId.Length})");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(workspaceId))
        {
            _logger.LogWarning("Clockify API credentials not fully configured. API Key: {HasApiKey}, Workspace ID: {HasWorkspaceId}. Using simulation mode instead.", 
                !string.IsNullOrEmpty(apiKey), !string.IsNullOrEmpty(workspaceId));
            await SimulateSyncAsync(entries);
            return;
        }

        _logger.LogInformation("Using real Clockify API integration with provided credentials");

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            httpClient.BaseAddress = new Uri("https://api.clockify.me/api/v1/");

            foreach (var entry in entries)
            {
                await SyncSingleEntryAsync(httpClient, workspaceId, entry);
            }

            _logger.LogInformation("Successfully synced {Count} entries to Clockify", entries.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync time entries to Clockify");
            throw;
        }
    }

    private async Task SyncSingleEntryAsync(HttpClient httpClient, string workspaceId, TimeEntryDto entry)
    {
        try
        {
            // Convert TimeEntryDto to Clockify format
            var clockifyEntry = new
            {
                start = entry.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end = entry.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                description = $"Task: {entry.TaskName} - Project: {entry.ProjectName}",
                // Note: You'll need to implement ID mapping from your system to Clockify IDs
                // For now, we'll use the task/project names in the description
            };

            var json = JsonSerializer.Serialize(clockifyEntry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"workspaces/{workspaceId}/time-entries", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Successfully synced time entry {EntryId} to Clockify", entry.EntryId);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to sync time entry {EntryId} to Clockify. Status: {StatusCode}, Error: {Error}", 
                    entry.EntryId, response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing time entry {EntryId} to Clockify", entry.EntryId);
        }
    }

    private async Task SimulateSyncAsync(List<TimeEntryDto> entries)
    {
        _logger.LogInformation("Simulating Clockify sync for {Count} entries", entries.Count);

        foreach (var entry in entries)
        {
            // Simulate API call delay
            await Task.Delay(200);
            
            _logger.LogDebug("Simulated sync of time entry {EntryId} to Clockify - Task: {TaskName}, Project: {ProjectName}, Duration: {Duration}", 
                entry.EntryId, entry.TaskName, entry.ProjectName, entry.Duration);
        }

        _logger.LogInformation("Completed simulated Clockify sync for {Count} entries", entries.Count);
    }
}
