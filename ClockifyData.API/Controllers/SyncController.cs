using Microsoft.AspNetCore.Mvc;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Services;

namespace ClockifyData.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;
    private readonly TimeEntryBatchSyncService _batchSyncService;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        ITimeEntryService timeEntryService,
        TimeEntryBatchSyncService batchSyncService,
        ILogger<SyncController> logger)
    {
        _timeEntryService = timeEntryService;
        _batchSyncService = batchSyncService;
        _logger = logger;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingSync([FromQuery] bool autoSync = false)
    {
        try
        {
            _logger.LogInformation("Getting pending sync items (autoSync: {AutoSync})", autoSync);
            
            // Get all time entries that might need syncing (last 7 days)
            var today = DateTime.Today;
            var oneWeekAgo = today.AddDays(-7);
            
            var timeEntries = await _timeEntryService.GetTimeEntriesByDateRangeAsync(oneWeekAgo, today);
            var timeEntriesList = timeEntries.ToList();
            
            if (autoSync && timeEntriesList.Any())
            {
                _logger.LogInformation("Auto-syncing {Count} pending entries to Clockify", timeEntriesList.Count);
                
                try
                {
                    await _batchSyncService.SyncToProviderAsync("Clockify", timeEntriesList);
                    
                    return Ok(new
                    {
                        message = "Pending sync items retrieved and synced to Clockify successfully",
                        count = timeEntriesList.Count,
                        synced = true,
                        provider = "Clockify",
                        items = timeEntriesList.Select(te => new
                        {
                            te.EntryId,
                            te.UserId,
                            te.TaskId,
                            te.StartTime,
                            te.EndTime,
                            Duration = te.EndTime - te.StartTime,
                            te.TaskName,
                            te.ProjectName,
                            te.UserName
                        })
                    });
                }
                catch (Exception syncEx)
                {
                    _logger.LogError(syncEx, "Failed to sync pending items to Clockify");
                    return Ok(new
                    {
                        message = "Pending sync items retrieved but sync to Clockify failed",
                        count = timeEntriesList.Count,
                        synced = false,
                        error = syncEx.Message,
                        items = timeEntriesList.Select(te => new
                        {
                            te.EntryId,
                            te.UserId,
                            te.TaskId,
                            te.StartTime,
                            te.EndTime,
                            Duration = te.EndTime - te.StartTime
                        })
                    });
                }
            }
            
            return Ok(new
            {
                message = "Pending sync items retrieved successfully",
                count = timeEntriesList.Count,
                synced = false,
                items = timeEntriesList.Select(te => new
                {
                    te.EntryId,
                    te.UserId,
                    te.TaskId,
                    te.StartTime,
                    te.EndTime,
                    Duration = te.EndTime - te.StartTime,
                    te.TaskName,
                    te.ProjectName,
                    te.UserName
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending sync items");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("sync-pending")]
    public async Task<IActionResult> SyncPendingToClockify()
    {
        try
        {
            _logger.LogInformation("Manually syncing pending items to Clockify");
            
            // Get all time entries that might need syncing (last 7 days)
            var today = DateTime.Today;
            var oneWeekAgo = today.AddDays(-7);
            
            var timeEntries = await _timeEntryService.GetTimeEntriesByDateRangeAsync(oneWeekAgo, today);
            var timeEntriesList = timeEntries.ToList();
            
            if (!timeEntriesList.Any())
            {
                return Ok(new
                {
                    message = "No pending sync items found",
                    count = 0,
                    synced = false
                });
            }
            
            await _batchSyncService.SyncToProviderAsync("Clockify", timeEntriesList);
            
            return Ok(new
            {
                message = $"Successfully synced {timeEntriesList.Count} pending items to Clockify",
                count = timeEntriesList.Count,
                synced = true,
                provider = "Clockify"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing pending items to Clockify");
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("test-clockify-config")]
    public async Task<IActionResult> TestClockifyConfig()
    {
        try
        {
            _logger.LogInformation("Testing Clockify configuration");
            
            // Create a fake time entry for testing
            var testEntry = new ClockifyData.Application.DTOs.TimeEntryDto
            {
                EntryId = 999,
                UserId = 1,
                TaskId = 1,
                StartTime = DateTime.Now.AddHours(-2),
                EndTime = DateTime.Now.AddHours(-1),
                UserName = "Test User",
                TaskName = "Test Task",
                ProjectName = "Test Project"
            };
            
            // Test the Clockify sync directly
            await _batchSyncService.SyncToProviderAsync("Clockify", new List<ClockifyData.Application.DTOs.TimeEntryDto> { testEntry });
            
            return Ok(new
            {
                message = "Clockify configuration test completed - check logs for details",
                testEntry = new
                {
                    testEntry.EntryId,
                    testEntry.TaskName,
                    testEntry.ProjectName,
                    testEntry.Duration
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Clockify configuration");
            return StatusCode(500, new { message = "Configuration test failed", error = ex.Message });
        }
    }

    [HttpGet("status")]
    public IActionResult GetSyncStatus()
    {
        try
        {
            var availableProviders = _batchSyncService.GetAvailableProviders().ToList();
            
            return Ok(new
            {
                status = "active",
                lastSync = DateTime.Now.AddHours(-2), // Mock data
                nextSync = DateTime.Now.AddHours(1),   // Mock data
                message = "Sync service is running",
                availableProviders = availableProviders
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sync status");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
