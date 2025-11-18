namespace BatteryService.Application.DTOs;

public class BatteryInventorySummaryDto
{
    public int TotalBatteries { get; set; }
    public int FullBatteries { get; set; }
    public int ChargingBatteries { get; set; }
    public int MaintenanceBatteries { get; set; }
    public int InUseBatteries { get; set; }
    public int AvailableBatteries { get; set; }
    public int DamagedBatteries { get; set; }
    public int RetiredBatteries { get; set; }
    public List<BatteryTypeInventoryDto> BatteryTypeInventories { get; set; }
    public List<BatteryCapacityInventoryDto> CapacityInventories { get; set; }
}

public class BatteryTypeInventoryDto
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

public class BatteryCapacityInventoryDto
{
    public decimal CapacityKwh { get; set; }
    public int TotalCount { get; set; }
    public int FullCount { get; set; }
    public int ChargingCount { get; set; }
    public int MaintenanceCount { get; set; }
}
