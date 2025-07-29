using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Mappings;
using System.Text.Json;
using System.Text;

namespace ClockifyData.Application.Services;

public class TimeEntryService : ITimeEntryService
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TimeEntryService> _logger;

    public TimeEntryService(
        ITimeEntryRepository timeEntryRepository,
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<TimeEntryService> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TimeEntryDto> AddTimeEntryAsync(CreateTimeEntryDto dto)
    {
        _logger.LogInformation("Creating new time entry for user {UserId}, task {TaskId}", dto.UserId, dto.TaskId);

        // Start transaction for data consistency
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {dto.UserId} not found");
            }

            // Validate task exists
            var task = await _taskRepository.GetByIdAsync(dto.TaskId);
            if (task == null)
            {
                throw new ArgumentException($"Task with ID {dto.TaskId} not found");
            }

            // Validate time range
            if (dto.EndTime <= dto.StartTime)
            {
                throw new ArgumentException("End time must be after start time");
            }

            var timeEntry = dto.ToEntity();

            // Try to sync to Clockify first
            try
            {
                var clockifyId = await SendToClockifyAsync(dto, task);
                timeEntry.ClockifyId = clockifyId;
                _logger.LogInformation("Time entry synced to Clockify with ID: {ClockifyId}", clockifyId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync time entry to Clockify, saving locally only");
            }

            var savedTimeEntry = await _timeEntryRepository.AddAsync(timeEntry);
            
            // Save changes to database using UnitOfWork
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            // Load navigation properties for DTO mapping
            savedTimeEntry.User = user;
            savedTimeEntry.Task = task;

            _logger.LogInformation("Time entry created successfully with ID: {EntryId}", savedTimeEntry.EntryId);
            
            return savedTimeEntry.ToDto();
        }
        catch
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByUserIdAsync(int userId)
    {
        var timeEntries = await _timeEntryRepository.GetTimeEntriesByUserIdAsync(userId);
        return timeEntries.Select(te => te.ToDto());
    }

    public async Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByDateRangeAsync(DateTime from, DateTime to)
    {
        var timeEntries = await _timeEntryRepository.GetTimeEntriesByDateRangeAsync(from, to);
        return timeEntries.Select(te => te.ToDto());
    }

    private async Task<string> SendToClockifyAsync(CreateTimeEntryDto dto, Domain.Entities.Task task)
    {
        var apiKey = _configuration["Clockify:ApiKey"];
        var workspaceId = _configuration["Clockify:WorkspaceId"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(workspaceId))
        {
            throw new InvalidOperationException("Clockify API credentials not configured");
        }

        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        httpClient.BaseAddress = new Uri("https://api.clockify.me/api/v1/");

        var timeEntryRequest = new
        {
            start = dto.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            end = dto.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            description = $"Task: {task.Name}"
        };

        var json = JsonSerializer.Serialize(timeEntryRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _logger.LogInformation("Sending time entry to Clockify: {Json}", json);
        
        var response = await httpClient.PostAsync($"workspaces/{workspaceId}/time-entries", content);

        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var clockifyResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);
            
            return clockifyResponse.GetProperty("id").GetString() ?? string.Empty;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to sync time entry to Clockify: {response.StatusCode} - {errorContent}");
        }
    }
}
