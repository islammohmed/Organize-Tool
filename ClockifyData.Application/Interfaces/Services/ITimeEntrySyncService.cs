using ClockifyData.Application.DTOs;

namespace ClockifyData.Application.Interfaces.Services;

public interface ITimeEntrySyncService
{
    Task SyncAsync(List<TimeEntryDto> entries);
    string ProviderName { get; }
}
