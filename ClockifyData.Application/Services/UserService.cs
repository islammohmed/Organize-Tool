using Microsoft.Extensions.Logging;
using ClockifyData.Application.DTOs;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Mappings;

namespace ClockifyData.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto dto)
    {
        _logger.LogInformation("Creating new user: {FullName}", dto.FullName);

        // Start transaction for data consistency
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = dto.ToEntity();
            var savedUser = await _userRepository.AddAsync(user);
            
            // Save changes to persist the user to the database
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("User created successfully with ID: {UserId}", savedUser.UserId);
            
            return savedUser.ToDto();
        }
        catch
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.ToDto();
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDto());
    }
}
