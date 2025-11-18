using BatteryService.Application.DTOs;

namespace BatteryService.Application.Services;

public interface IBatteryInventoryService
{
    Task<List<BatteryInventoryDto>> GetBatteryInventoryAsync(BatteryFilterDto filter);
    Task<List<BatteryInventoryDto>> GetAllBatteriesAsync();
    Task<BatteryInventoryDto> GetBatteryDetailAsync(Guid batteryId);
    Task<BatteryInventorySummaryDto> GetInventorySummaryAsync(Guid? stationId = null);
    Task<bool> UpdateBatteryStatusAsync(Guid batteryId, string status);
    Task<bool> UpdateBatteryChargeLevelAsync(Guid batteryId, int chargeLevel);
}
