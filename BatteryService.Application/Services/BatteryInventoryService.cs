using BatteryService.Application.DTOs;
using BatteryService.Domain.Repositories;

namespace BatteryService.Application.Services;

public class BatteryInventoryService : IBatteryInventoryService
{
    private readonly IBatteryRepository _batteryRepository;

    public BatteryInventoryService(IBatteryRepository batteryRepository)
    {
        _batteryRepository = batteryRepository;
    }

    public async Task<List<BatteryInventoryDto>> GetBatteryInventoryAsync(BatteryFilterDto filter)
    {
        var batteries = await _batteryRepository.GetBatteriesAsync(
            statuses: filter?.Statuses,
            batteryTypeIds: filter?.BatteryTypeIds,
            minCapacity: filter?.MinCapacityKwh,
            maxCapacity: filter?.MaxCapacityKwh,
            minChargeLevel: filter?.MinChargeLevel,
            maxChargeLevel: filter?.MaxChargeLevel,
            minSoh: filter?.MinSohPercentage,
            manufacturer: filter?.Manufacturer,
            stationId: filter?.StationId,
            searchText: filter?.SearchText
        );

        return batteries.Select(b => new BatteryInventoryDto
        {
            BatteryId = b.BatteryId,
            BatteryCode = b.BatteryCode,
            SerialNumber = b.SerialNumber,
            Status = b.Status,
            ChargeLevel = b.ChargeLevel,
            SohPercentage = b.SohPercentage,
            TotalCycles = b.TotalCycles,
            BatteryTypeId = b.BatteryTypeId,
            TypeCode = b.BatteryType?.TypeCode,
            TypeName = b.BatteryType?.TypeName,
            Manufacturer = b.BatteryType?.Manufacturer,
            CapacityKwh = b.BatteryType?.CapacityKwh ?? 0,
            LastMaintenanceDate = b.LastMaintenanceDate,
            NextMaintenanceDate = b.NextMaintenanceDate,
            LastSwapDate = b.LastSwapDate,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();
    }

    public async Task<List<BatteryInventoryDto>> GetAllBatteriesAsync()
    {
        var batteries = await _batteryRepository.GetBatteriesAsync();
        
        return batteries.Select(b => new BatteryInventoryDto
        {
            BatteryId = b.BatteryId,
            BatteryCode = b.BatteryCode,
            SerialNumber = b.SerialNumber,
            Status = b.Status,
            ChargeLevel = b.ChargeLevel,
            SohPercentage = b.SohPercentage,
            TotalCycles = b.TotalCycles,
            BatteryTypeId = b.BatteryTypeId,
            TypeCode = b.BatteryType?.TypeCode,
            TypeName = b.BatteryType?.TypeName,
            Manufacturer = b.BatteryType?.Manufacturer,
            CapacityKwh = b.BatteryType?.CapacityKwh ?? 0,
            LastMaintenanceDate = b.LastMaintenanceDate,
            NextMaintenanceDate = b.NextMaintenanceDate,
            LastSwapDate = b.LastSwapDate,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();
    }

    public async Task<BatteryInventoryDto> GetBatteryDetailAsync(Guid batteryId)
    {
        var battery = await _batteryRepository.GetBatteryByIdAsync(batteryId);
        
        if (battery == null)
            return null;

        return new BatteryInventoryDto
        {
            BatteryId = battery.BatteryId,
            BatteryCode = battery.BatteryCode,
            SerialNumber = battery.SerialNumber,
            Status = battery.Status,
            ChargeLevel = battery.ChargeLevel,
            SohPercentage = battery.SohPercentage,
            TotalCycles = battery.TotalCycles,
            BatteryTypeId = battery.BatteryTypeId,
            TypeCode = battery.BatteryType?.TypeCode,
            TypeName = battery.BatteryType?.TypeName,
            Manufacturer = battery.BatteryType?.Manufacturer,
            CapacityKwh = battery.BatteryType?.CapacityKwh ?? 0,
            LastMaintenanceDate = battery.LastMaintenanceDate,
            NextMaintenanceDate = battery.NextMaintenanceDate,
            LastSwapDate = battery.LastSwapDate,
            CreatedAt = battery.CreatedAt,
            UpdatedAt = battery.UpdatedAt
        };
    }

    public async Task<BatteryInventorySummaryDto> GetInventorySummaryAsync()
    {
        var statusCounts = await _batteryRepository.GetBatteryCountByStatusAsync();
        var typeStatistics = await _batteryRepository.GetBatteryStatisticsByTypeAsync();
        var capacityStatistics = await _batteryRepository.GetBatteryStatisticsByCapacityAsync();

        return new BatteryInventorySummaryDto
        {
            TotalBatteries = statusCounts.Values.Sum(),
            FullBatteries = statusCounts.GetValueOrDefault("available", 0),
            ChargingBatteries = statusCounts.GetValueOrDefault("charging", 0),
            MaintenanceBatteries = statusCounts.GetValueOrDefault("maintenance", 0),
            InUseBatteries = statusCounts.GetValueOrDefault("in_use", 0),
            AvailableBatteries = statusCounts.GetValueOrDefault("available", 0),
            DamagedBatteries = statusCounts.GetValueOrDefault("faulty", 0),
            RetiredBatteries = statusCounts.GetValueOrDefault("retired", 0),
            BatteryTypeInventories = typeStatistics.Select(t => new BatteryTypeInventoryDto
            {
                BatteryTypeId = t.BatteryTypeId,
                TypeCode = t.TypeCode,
                TypeName = t.TypeName,
                Manufacturer = t.Manufacturer,
                CapacityKwh = t.CapacityKwh,
                TotalCount = t.TotalCount,
                FullCount = t.FullCount,
                ChargingCount = t.ChargingCount,
                MaintenanceCount = t.MaintenanceCount
            }).ToList(),
            CapacityInventories = capacityStatistics.Select(c => new BatteryCapacityInventoryDto
            {
                CapacityKwh = c.CapacityKwh,
                TotalCount = c.TotalCount,
                FullCount = c.FullCount,
                ChargingCount = c.ChargingCount,
                MaintenanceCount = c.MaintenanceCount
            }).ToList()
        };
    }

    public async Task<bool> UpdateBatteryStatusAsync(Guid batteryId, string status)
    {
        return await _batteryRepository.UpdateBatteryStatusAsync(batteryId, status);
    }

    public async Task<bool> UpdateBatteryChargeLevelAsync(Guid batteryId, int chargeLevel)
    {
        if (chargeLevel < 0 || chargeLevel > 100)
            return false;

        return await _batteryRepository.UpdateBatteryChargeLevelAsync(batteryId, chargeLevel);
    }
}
