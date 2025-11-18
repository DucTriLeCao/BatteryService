using BatteryService.Domain.Models;

namespace BatteryService.Domain.Repositories;

public interface IBatteryRepository
{
    Task<List<Battery>> GetBatteriesAsync(
        List<string> statuses = null,
        List<Guid> batteryTypeIds = null,
        decimal? minCapacity = null,
        decimal? maxCapacity = null,
        int? minChargeLevel = null,
        int? maxChargeLevel = null,
        decimal? minSoh = null,
        string manufacturer = null,
        Guid? stationId = null,
        string searchText = null);

    Task<Battery> GetBatteryByIdAsync(Guid batteryId);
    Task<Dictionary<string, int>> GetBatteryCountByStatusAsync(Guid? stationId = null);
    Task<List<BatteryTypeStatistics>> GetBatteryStatisticsByTypeAsync(Guid? stationId = null);
    Task<List<BatteryCapacityStatistics>> GetBatteryStatisticsByCapacityAsync(Guid? stationId = null);
    Task<bool> UpdateBatteryStatusAsync(Guid batteryId, string status);
    Task<bool> UpdateBatteryChargeLevelAsync(Guid batteryId, int chargeLevel);
}

public class BatteryTypeStatistics
{
    public Guid BatteryTypeId { get; set; }
    public string TypeCode { get; set; }
    public string TypeName { get; set; }
    public string Manufacturer { get; set; }
    public decimal CapacityKwh { get; set; }
    public int TotalCount { get; set; }
    public int FullCount { get; set; }
    public int ChargingCount { get; set; }
    public int MaintenanceCount { get; set; }
}

public class BatteryCapacityStatistics
{
    public decimal CapacityKwh { get; set; }
    public int TotalCount { get; set; }
    public int FullCount { get; set; }
    public int ChargingCount { get; set; }
    public int MaintenanceCount { get; set; }
}
