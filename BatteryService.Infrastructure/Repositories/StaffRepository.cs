using BatteryService.Domain.Models;
using BatteryService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BatteryService.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly ev_battery_swapContext _context;

    public StaffRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<Staff> GetStaffByUserIdAsync(Guid userId)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
