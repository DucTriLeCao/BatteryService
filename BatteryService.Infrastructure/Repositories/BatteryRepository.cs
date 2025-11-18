using BatteryService.Domain.Models;
using BatteryService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BatteryService.Infrastructure.Repositories;

public class BatteryRepository : IBatteryRepository
{
    private readonly ev_battery_swapContext _context;

    public BatteryRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<List<Battery>> GetBatteriesAsync(
        List<string> statuses = null,
        List<Guid> batteryTypeIds = null,
        decimal? minCapacity = null,
        decimal? maxCapacity = null,
        int? minChargeLevel = null,
        int? maxChargeLevel = null,
        decimal? minSoh = null,
        string manufacturer = null,
        Guid? stationId = null,
        string searchText = null)
    {
        var query = _context.Batteries
            .Include(b => b.BatteryType)
            .AsQueryable();

        if (statuses != null && statuses.Any())
        {
            query = query.Where(b => statuses.Any(s => b.Status.Contains(s)));
        }

        if (batteryTypeIds != null && batteryTypeIds.Any())
        {
            query = query.Where(b => batteryTypeIds.Any(id => b.BatteryType.TypeCode.Contains(id.ToString()) || b.BatteryType.TypeName.Contains(id.ToString())));
        }

        if (minCapacity.HasValue)
        {
            query = query.Where(b => b.BatteryType.CapacityKwh >= minCapacity.Value);
        }

        if (maxCapacity.HasValue)
        {
            query = query.Where(b => b.BatteryType.CapacityKwh <= maxCapacity.Value);
        }

        if (minChargeLevel.HasValue)
        {
            query = query.Where(b => b.ChargeLevel >= minChargeLevel.Value);
        }

        if (maxChargeLevel.HasValue)
        {
            query = query.Where(b => b.ChargeLevel <= maxChargeLevel.Value);
        }

        if (minSoh.HasValue)
        {
            query = query.Where(b => b.SohPercentage >= minSoh.Value);
        }

        if (!string.IsNullOrEmpty(manufacturer))
        {
            query = query.Where(b => b.BatteryType.Manufacturer.Contains(manufacturer));
        }

        if (stationId.HasValue)
        {
            query = query.Where(b => b.StationId == stationId.Value);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(b => 
                b.BatteryCode.Contains(searchText) || 
                b.SerialNumber.Contains(searchText));
        }

        return await query.OrderBy(b => b.Status)
                          .ThenByDescending(b => b.ChargeLevel)
                          .ToListAsync();
    }

    public async Task<Battery> GetBatteryByIdAsync(Guid batteryId)
    {
        return await _context.Batteries
            .Include(b => b.BatteryType)
            .Include(b => b.BatteryHealthLogs)
            .FirstOrDefaultAsync(b => b.BatteryId == batteryId);
    }

    public async Task<Dictionary<string, int>> GetBatteryCountByStatusAsync()
    {
        return await _context.Batteries
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<List<BatteryTypeStatistics>> GetBatteryStatisticsByTypeAsync()
    {
        return await _context.Batteries
            .Include(b => b.BatteryType)
            .GroupBy(b => new
            {
                b.BatteryTypeId,
                b.BatteryType.TypeCode,
                b.BatteryType.TypeName,
                b.BatteryType.Manufacturer,
                b.BatteryType.CapacityKwh
            })
            .Select(g => new BatteryTypeStatistics
            {
                BatteryTypeId = g.Key.BatteryTypeId,
                TypeCode = g.Key.TypeCode,
                TypeName = g.Key.TypeName,
                Manufacturer = g.Key.Manufacturer,
                CapacityKwh = g.Key.CapacityKwh,
                TotalCount = g.Count(),
                FullCount = g.Count(b => b.Status == "Full"),
                ChargingCount = g.Count(b => b.Status == "Charging"),
                MaintenanceCount = g.Count(b => b.Status == "Maintenance")
            })
            .OrderBy(s => s.CapacityKwh)
            .ThenBy(s => s.TypeName)
            .ToListAsync();
    }

    public async Task<List<BatteryCapacityStatistics>> GetBatteryStatisticsByCapacityAsync()
    {
        return await _context.Batteries
            .Include(b => b.BatteryType)
            .GroupBy(b => b.BatteryType.CapacityKwh)
            .Select(g => new BatteryCapacityStatistics
            {
                CapacityKwh = g.Key,
                TotalCount = g.Count(),
                FullCount = g.Count(b => b.Status == "Full"),
                ChargingCount = g.Count(b => b.Status == "Charging"),
                MaintenanceCount = g.Count(b => b.Status == "Maintenance")
            })
            .OrderBy(s => s.CapacityKwh)
            .ToListAsync();
    }

    public async Task<bool> UpdateBatteryStatusAsync(Guid batteryId, string status)
    {
        var battery = await _context.Batteries.FindAsync(batteryId);
        if (battery == null)
            return false;

        battery.Status = status;
        battery.UpdatedAt = DateTime.UtcNow;
        
        _context.Batteries.Update(battery);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateBatteryChargeLevelAsync(Guid batteryId, int chargeLevel)
    {
        var battery = await _context.Batteries.FindAsync(batteryId);
        if (battery == null)
            return false;

        battery.ChargeLevel = chargeLevel;
        battery.UpdatedAt = DateTime.UtcNow;
        
        if (chargeLevel >= 95)
        {
            battery.Status = "available";
        }
        else if (chargeLevel < 95 && chargeLevel > 20)
        {
            battery.Status = "charging";
        }
        
        _context.Batteries.Update(battery);
        await _context.SaveChangesAsync();
        return true;
    }
}
