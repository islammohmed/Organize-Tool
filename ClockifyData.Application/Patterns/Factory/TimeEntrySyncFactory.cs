using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ClockifyData.Application.Interfaces.Services;

namespace ClockifyData.Application.Patterns.Factory;

public class TimeEntrySyncFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TimeEntrySyncFactory> _logger;
    private readonly Dictionary<string, Type> _syncServices;

    public TimeEntrySyncFactory(
        IServiceProvider serviceProvider,
        ILogger<TimeEntrySyncFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _syncServices = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        
        // Register available sync services
        RegisterSyncServices();
    }

    public ITimeEntrySyncService Create(string providerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);

        _logger.LogDebug("Creating TimeEntrySyncService for provider: {ProviderName}", providerName);

        if (!_syncServices.TryGetValue(providerName, out var serviceType))
        {
            var availableProviders = string.Join(", ", _syncServices.Keys);
            throw new NotSupportedException(
                $"Time entry sync provider '{providerName}' is not supported. " +
                $"Available providers: {availableProviders}");
        }

        var service = _serviceProvider.GetService(serviceType) as ITimeEntrySyncService;
        
        if (service == null)
        {
            throw new InvalidOperationException(
                $"Failed to create sync service for provider '{providerName}'. " +
                $"Ensure the service is registered in dependency injection.");
        }

        _logger.LogInformation("Created TimeEntrySyncService for provider: {ProviderName}", providerName);
        return service;
    }

    public IEnumerable<string> GetAvailableProviders()
    {
        return _syncServices.Keys.ToList();
    }

    private void RegisterSyncServices()
    {
        // Register all available sync service implementations
        // This method discovers sync services from the DI container
        var syncServices = _serviceProvider.GetServices<ITimeEntrySyncService>();
        
        foreach (var service in syncServices)
        {
            var providerName = service.ProviderName;
            var serviceType = service.GetType();
            
            _syncServices[providerName] = serviceType;
            _logger.LogDebug("Registered TimeEntrySyncService: {ProviderName} -> {ServiceType}", 
                providerName, serviceType.Name);
        }

        _logger.LogInformation("Registered {Count} time entry sync providers: {Providers}", 
            _syncServices.Count, string.Join(", ", _syncServices.Keys));
    }
}
