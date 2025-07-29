using Microsoft.AspNetCore.Mvc;
using ClockifyData.Application.Services;
using ClockifyData.Application.Patterns.Factory;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.DTOs;

namespace ClockifyData.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntrySyncController : ControllerBase
{
    private readonly TimeEntryBatchSyncService _batchSyncService;
    private readonly TimeEntrySyncFactory _syncFactory;
    private readonly ITimeEntryService _timeEntryService;
    private readonly ILogger<TimeEntrySyncController> _logger;

    public TimeEntrySyncController(
        TimeEntryBatchSyncService batchSyncService,
        TimeEntrySyncFactory syncFactory,
        ITimeEntryService timeEntryService,
        ILogger<TimeEntrySyncController> logger)
    {
        _batchSyncService = batchSyncService;
        _syncFactory = syncFactory;
        _timeEntryService = timeEntryService;
        _logger = logger;
    }

    [HttpGet("providers")]
    public ActionResult<IEnumerable<string>> GetAvailableProviders()
    {
        try
        {
            var providers = _batchSyncService.GetAvailableProviders();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available sync providers");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("sync/{providerName}")]
    public async Task<IActionResult> SyncToProvider(
        string providerName,
        [FromBody] List<TimeEntryDto> entries)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return BadRequest(new { message = "Provider name is required" });
            }

            if (entries == null || !entries.Any())
            {
                return BadRequest(new { message = "Time entries are required" });
            }

            await _batchSyncService.SyncToProviderAsync(providerName, entries);
            
            return Ok(new { 
                message = $"Successfully synced {entries.Count} entries to {providerName}",
                provider = providerName,
                entryCount = entries.Count
            });
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing to provider {Provider}", providerName);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("sync/multiple")]
    public async Task<IActionResult> SyncToMultipleProviders(
        [FromBody] MultiProviderSyncRequest request)
    {
        try
        {
            if (request.ProviderNames == null || !request.ProviderNames.Any())
            {
                return BadRequest(new { message = "At least one provider name is required" });
            }

            if (request.Entries == null || !request.Entries.Any())
            {
                return BadRequest(new { message = "Time entries are required" });
            }

            await _batchSyncService.SyncToMultipleProvidersAsync(request.ProviderNames, request.Entries);
            
            return Ok(new { 
                message = $"Completed multi-provider sync to {request.ProviderNames.Count} providers",
                providers = request.ProviderNames,
                entryCount = request.Entries.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in multi-provider sync");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("sync/user/{userId}/date-range")]
    public async Task<IActionResult> SyncUserEntriesInDateRange(
        int userId,
        string providerName,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return BadRequest(new { message = "Provider name is required" });
            }

            if (from > to)
            {
                return BadRequest(new { message = "From date must be before to date" });
            }

            // Get user's time entries in the specified date range
            var timeEntries = await _timeEntryService.GetTimeEntriesByDateRangeAsync(from, to);
            var userEntries = timeEntries.Where(te => te.UserId == userId).ToList();

            if (!userEntries.Any())
            {
                return Ok(new { 
                    message = "No time entries found for the specified user and date range",
                    userId,
                    dateRange = new { from, to }
                });
            }

            await _batchSyncService.SyncToProviderAsync(providerName, userEntries);
            
            return Ok(new { 
                message = $"Successfully synced {userEntries.Count} entries for user {userId} to {providerName}",
                userId,
                provider = providerName,
                entryCount = userEntries.Count,
                dateRange = new { from, to }
            });
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user entries to provider {Provider}", providerName);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class MultiProviderSyncRequest
{
    public List<string> ProviderNames { get; set; } = new();
    public List<TimeEntryDto> Entries { get; set; } = new();
}
