using Microsoft.Extensions.DependencyInjection;
using ClockifyData.Application.Interfaces.Repositories;
using ClockifyData.Infrastructure.Repositories.Implementations;

namespace ClockifyData.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

        return services;
    }
}
