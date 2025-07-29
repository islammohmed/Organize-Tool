using Microsoft.Extensions.Logging;
using ClockifyData.Application.Interfaces.Services;
using ClockifyData.Application.Patterns.Factory;
using ClockifyData.Application.DTOs;

namespace ClockifyData.Application.Services;

public class TimeEntryBatchSyncService
{
    private readonly TimeEntrySyncFactory _syncFactory;
    private readonly ILogger<TimeEntryBatchSyncService> _logger;

    public TimeEntryBatchSyncService(
        TimeEntrySyncFactory syncFactory,
        ILogger<TimeEntryBatchSyncService> logger)
    {
        _syncFactory = syncFactory;
        _logger = logger;
    }

    public async Task SyncToProviderAsync(string providerName, List<TimeEntryDto> entries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        ArgumentNullException.ThrowIfNull(entries);

        _logger.LogInformation("Starting batch sync of {Count} entries to provider: {Provider}", 
            entries.Count, providerName);

        try
        {
            // Use factory to create the appropriate sync service
            var syncService = _syncFactory.Create(providerName);
            
            // Perform the sync
            await syncService.SyncAsync(entries);
            
            _logger.LogInformation("Successfully completed batch sync to {Provider}", providerName);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "Unsupported sync provider: {Provider}", providerName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync entries to provider: {Provider}", providerName);
            throw;
        }
    }

    public async Task SyncToMultipleProvidersAsync(List<string> providerNames, List<TimeEntryDto> entries)
    {
        ArgumentNullException.ThrowIfNull(providerNames);
        ArgumentNullException.ThrowIfNull(entries);

        _logger.LogInformation("Starting multi-provider sync to {ProviderCount} providers for {EntryCount} entries", 
            providerNames.Count, entries.Count);

        var tasks = providerNames.Select(async providerName =>
        {
            try
            {
                await SyncToProviderAsync(providerName, entries);
                return new { Provider = providerName, Success = true, Error = (Exception?)null };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync to provider: {Provider}", providerName);
                return new { Provider = providerName, Success = false, Error = (Exception?)ex };
            }
        });

        var results = await Task.WhenAll(tasks);
        
        var successCount = results.Count(r => r.Success);
        var failureCount = results.Count(r => !r.Success);
        
        _logger.LogInformation("Multi-provider sync completed: {SuccessCount} successful, {FailureCount} failed", 
            successCount, failureCount);

        // Log failed providers
        foreach (var failure in results.Where(r => !r.Success))
        {
            _logger.LogWarning("Provider {Provider} failed: {Error}", failure.Provider, failure.Error?.Message);
        }
    }

    public IEnumerable<string> GetAvailableProviders()
    {
        return _syncFactory.GetAvailableProviders();
    }
}
