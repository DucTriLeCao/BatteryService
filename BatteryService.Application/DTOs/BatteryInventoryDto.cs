using BatteryService.Domain.Enums;

namespace BatteryService.Application.DTOs;

public class BatteryInventoryDto
{
    public Guid BatteryId { get; set; }
    public string BatteryCode { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
    public int ChargeLevel { get; set; }
    public decimal SohPercentage { get; set; }
    public int TotalCycles { get; set; }
    
    public Guid BatteryTypeId { get; set; }
    public string TypeCode { get; set; }
    public string TypeName { get; set; }
    public string Manufacturer { get; set; }
    public decimal CapacityKwh { get; set; }
    
    public DateOnly? LastMaintenanceDate { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }
    public DateTime? LastSwapDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
