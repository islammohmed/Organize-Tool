using Microsoft.Extensions.Logging;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Mappings;

namespace ClockifyData.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<ProjectDto> AddProjectAsync(CreateProjectDto dto)
    {
        _logger.LogInformation("Creating new project: {ProjectName} for user: {UserId}", dto.Name, dto.UserId);

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

            var project = dto.ToEntity();
            var savedProject = await _projectRepository.AddAsync(project);
            
            // Save changes to database
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            // Load user for DTO mapping
            savedProject.User = user;

            _logger.LogInformation("Project created successfully with ID: {ProjectId}", savedProject.ProjectId);
            
            return savedProject.ToDto();
        }
        catch
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async System.Threading.Tasks.Task<ProjectDto?> GetProjectByIdAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        return project?.ToDto();
    }

    public async System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(p => p.ToDto());
    }
}
