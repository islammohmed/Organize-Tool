using Microsoft.AspNetCore.Mvc;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.DTOs;

namespace ClockifyData.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(IAssignmentService assignmentService, ILogger<AssignmentsController> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AssignTask([FromBody] AssignTaskDto dto)
    {
        try
        {
            await _assignmentService.AssignTaskToUserAsync(dto);
            return Ok(new { message = "Task assigned successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user/{userId}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksAssignedToUser(int userId)
    {
        try
        {
            var tasks = await _assignmentService.GetTasksAssignedToUserAsync(userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for user {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("task/{taskId}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAssignedToTask(int taskId)
    {
        try
        {
            var users = await _assignmentService.GetUsersAssignedToTaskAsync(taskId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for task {TaskId}", taskId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
