using Microsoft.AspNetCore.Mvc;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClockifyData.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TimeEntriesController> _logger;

    public TimeEntriesController(
        ITimeEntryService timeEntryService,
        ApplicationDbContext dbContext,
        ILogger<TimeEntriesController> logger)
    {
        _timeEntryService = timeEntryService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTimeEntries(
        [FromQuery] int? userId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            IEnumerable<TimeEntryDto> timeEntries;

            if (userId.HasValue)
            {
                timeEntries = await _timeEntryService.GetTimeEntriesByUserIdAsync(userId.Value);
            }
            else if (from.HasValue && to.HasValue)
            {
                timeEntries = await _timeEntryService.GetTimeEntriesByDateRangeAsync(from.Value, to.Value);
            }
            else
            {
                return BadRequest(new { message = "Please provide either userId or both from and to dates" });
            }

            return Ok(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllTimeEntries()
    {
        try
        {
            // Get all time entries directly from the database without date filtering
            var allTimeEntries = await _dbContext.TimeEntries
                .Include(te => te.User)
                .Include(te => te.Task)
                    .ThenInclude(t => t.Project)
                .ToListAsync();
                
            _logger.LogInformation("Found {Count} total time entries in database", allTimeEntries.Count);
            
            if (allTimeEntries.Any())
            {
                _logger.LogInformation("Date range in database: {EarliestDate} to {LatestDate}",
                    allTimeEntries.Min(te => te.StartTime),
                    allTimeEntries.Max(te => te.EndTime));
            }
            
            var timeEntriesData = allTimeEntries.Select(entry => new
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                UserName = entry.User?.FullName ?? "Unknown",
                TaskId = entry.TaskId,
                TaskName = entry.Task?.Name ?? "Unknown",
                ProjectName = entry.Task?.Project?.Name ?? "Unknown",
                StartTime = entry.StartTime,
                EndTime = entry.EndTime,
                Duration = (entry.EndTime - entry.StartTime).TotalHours
            }).ToList();
            
            return Ok(timeEntriesData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all time entries");
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<TimeEntryDto>> CreateTimeEntry([FromBody] CreateTimeEntryDto dto)
    {
        try
        {
            var timeEntry = await _timeEntryService.AddTimeEntryAsync(dto);
            return CreatedAtAction(nameof(GetTimeEntriesByUser), new { userId = timeEntry.UserId }, timeEntry);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating time entry");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByUser(int userId)
    {
        try
        {
            var timeEntries = await _timeEntryService.GetTimeEntriesByUserIdAsync(userId);
            return Ok(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time entries for user {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByDateRange(
        [FromQuery] DateTime from, 
        [FromQuery] DateTime to)
    {
        try
        {
            if (from > to)
            {
                return BadRequest(new { message = "From date must be before to date" });
            }

            var timeEntries = await _timeEntryService.GetTimeEntriesByDateRangeAsync(from, to);
            return Ok(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time entries for date range {From} to {To}", from, to);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
