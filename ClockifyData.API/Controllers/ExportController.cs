using Microsoft.AspNetCore.Mvc;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClockifyData.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public ExportController(IExportService exportService, ILogger<ExportController> logger, ApplicationDbContext dbContext)
    {
        _exportService = exportService;
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet("report/csv")]
    public async Task<IActionResult> ExportReportToCsv(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            // If dates not provided, use current month
            var currentDate = DateTime.UtcNow;
            var fromDate = from ?? new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = to ?? new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            
            if (fromDate > toDate)
            {
                return BadRequest(new { message = "From date must be before to date" });
            }

            _logger.LogInformation("Exporting report from {From:yyyy-MM-dd} to {To:yyyy-MM-dd}", fromDate, toDate);
            
            var csvData = await _exportService.ExportReportToCsvAsync(fromDate, toDate);
            
            var fileName = $"time_entries_report_{fromDate:yyyy-MM-dd}_to_{toDate:yyyy-MM-dd}.csv";
            
            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to CSV");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("current-month/csv")]
    public async Task<IActionResult> ExportCurrentMonthReportToCsv()
    {
        try
        {
            var csvData = await _exportService.ExportCurrentMonthReportToCsvAsync();
            
            var currentDate = DateTime.UtcNow;
            var fileName = $"monthly_report_{currentDate:yyyy-MM}.csv";
            
            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting current month report to CSV");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
