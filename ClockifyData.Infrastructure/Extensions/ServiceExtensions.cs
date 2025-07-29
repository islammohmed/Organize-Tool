using Microsoft.Extensions.DependencyInjection;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Interfaces.UnitOfWork;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Application.Services;
using ClockifyData.Application.Patterns.Factory;
using ClockifyData.Infrastructure.Repositories;

namespace ClockifyData.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Add HttpClient factory
        services.AddHttpClient();
        
        // Add business services
        services.AddScoped<IProjectService, Application.Services.ProjectService>();
        services.AddScoped<ITaskService, Application.Services.TaskService>();
        services.AddScoped<IUserService, Application.Services.UserService>();
        services.AddScoped<IAssignmentService, Application.Services.AssignmentService>();
        services.AddScoped<ITimeEntryService, Application.Services.TimeEntryService>();
        services.AddScoped<IExportService, Application.Services.ExportService>();
        
        // Add sync services - Clockify only
        services.AddScoped<Application.Services.ClockifyTimeEntrySyncService>();
        services.AddScoped<ITimeEntrySyncService, Application.Services.ClockifyTimeEntrySyncService>();
        services.AddScoped<TimeEntryBatchSyncService>();
        services.AddScoped<TimeEntrySyncFactory>();
        
        // UserTaskRepository removed - assignments handled via direct Task-User relationship
        
        // Core infrastructure services
        services.AddScoped<IUnitOfWork, ClockifyData.Infrastructure.UnitOfWork.UnitOfWork>();

        return services;
    }
}
