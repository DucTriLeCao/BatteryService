using BatteryService.Domain.Models;

namespace BatteryService.Domain.Repositories;

public interface IStaffRepository
{
    Task<Staff> GetStaffByUserIdAsync(Guid userId);
}
