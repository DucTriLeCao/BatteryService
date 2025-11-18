namespace BatteryService.Application.DTOs;

public class BatteryFilterDto
{
    public List<string> Statuses { get; set; }
    public List<Guid> BatteryTypeIds { get; set; }
    public decimal? MinCapacityKwh { get; set; }
    public decimal? MaxCapacityKwh { get; set; }
    public int? MinChargeLevel { get; set; }
    public int? MaxChargeLevel { get; set; }
    public decimal? MinSohPercentage { get; set; }
    public string Manufacturer { get; set; }
    public Guid? StationId { get; set; }
    public string SearchText { get; set; }
}
