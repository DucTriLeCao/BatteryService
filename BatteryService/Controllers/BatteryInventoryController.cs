using BatteryService.Application.DTOs;
using BatteryService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff")]
public class BatteryInventoryController : ControllerBase
{
    private readonly IBatteryInventoryService _inventoryService;
    private readonly ILogger<BatteryInventoryController> _logger;

    public BatteryInventoryController(
        IBatteryInventoryService inventoryService,
        ILogger<BatteryInventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<BatteryInventorySummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventorySummary()
    {
        try
        {
            var summary = await _inventoryService.GetInventorySummaryAsync();
            return Ok(ApiResponse<BatteryInventorySummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory summary");
            return BadRequest(ApiResponse<BatteryInventorySummaryDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<List<BatteryInventoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBatteries()
    {
        try
        {
            var batteries = await _inventoryService.GetAllBatteriesAsync();
            return Ok(ApiResponse<List<BatteryInventoryDto>>.SuccessResponse(batteries));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all batteries");
            return BadRequest(ApiResponse<List<BatteryInventoryDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("{batteryId}")]
    [ProducesResponseType(typeof(ApiResponse<BatteryInventoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBatteryDetail(Guid batteryId)
    {
        try
        {
            var battery = await _inventoryService.GetBatteryDetailAsync(batteryId);
            if (battery == null)
            {
                return NotFound(ApiResponse<BatteryInventoryDto>.ErrorResponse("Battery not found"));
            }

            return Ok(ApiResponse<BatteryInventoryDto>.SuccessResponse(battery));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting battery detail for {BatteryId}", batteryId);
            return BadRequest(ApiResponse<BatteryInventoryDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponse<List<BatteryInventoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableBatteries()
    {
        try
        {
            var filter = new BatteryFilterDto
            {
                Statuses = new List<string> { "available" }
            };

            var batteries = await _inventoryService.GetBatteryInventoryAsync(filter);
            return Ok(ApiResponse<List<BatteryInventoryDto>>.SuccessResponse(batteries));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting full batteries");
            return BadRequest(ApiResponse<List<BatteryInventoryDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("charging")]
    [ProducesResponseType(typeof(ApiResponse<List<BatteryInventoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChargingBatteries()
    {
        try
        {
            var filter = new BatteryFilterDto
            {
                Statuses = new List<string> { "charging" }
            };

            var batteries = await _inventoryService.GetBatteryInventoryAsync(filter);
            return Ok(ApiResponse<List<BatteryInventoryDto>>.SuccessResponse(batteries));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting charging batteries");
            return BadRequest(ApiResponse<List<BatteryInventoryDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("maintenance")]
    [ProducesResponseType(typeof(ApiResponse<List<BatteryInventoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaintenanceBatteries()
    {
        try
        {
            var filter = new BatteryFilterDto
            {
                Statuses = new List<string> { "maintenance" }
            };

            var batteries = await _inventoryService.GetBatteryInventoryAsync(filter);
            return Ok(ApiResponse<List<BatteryInventoryDto>>.SuccessResponse(batteries));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting maintenance batteries");
            return BadRequest(ApiResponse<List<BatteryInventoryDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<BatteryInventoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchBatteries(
        [FromQuery] string? status = null,
        [FromQuery] Guid? batteryTypeId = null,
        [FromQuery] decimal? minCapacity = null,
        [FromQuery] decimal? maxCapacity = null)
    {
        try
        {
            var filter = new BatteryFilterDto
            {
                Statuses = string.IsNullOrEmpty(status) ? null : new List<string> { status },
                BatteryTypeIds = batteryTypeId.HasValue ? new List<Guid> { batteryTypeId.Value } : null,
                MinCapacityKwh = minCapacity,
                MaxCapacityKwh = maxCapacity
            };

            var batteries = await _inventoryService.GetBatteryInventoryAsync(filter);
            return Ok(ApiResponse<List<BatteryInventoryDto>>.SuccessResponse(batteries));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching batteries");
            return BadRequest(ApiResponse<List<BatteryInventoryDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{batteryId}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBatteryStatus(
        Guid batteryId,
        [FromBody] UpdateStatusRequest request)
    {
        try
        {
            _logger.LogInformation("Updating battery {BatteryId} status to {Status}", batteryId, request.Status);

            var result = await _inventoryService.UpdateBatteryStatusAsync(batteryId, request.Status);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Battery not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Battery status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating battery status for {BatteryId}", batteryId);
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{batteryId}/charge-level")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBatteryChargeLevel(
        Guid batteryId,
        [FromBody] UpdateChargeLevelRequest request)
    {
        try
        {
            if (request.ChargeLevel < 0 || request.ChargeLevel > 100)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Charge level must be between 0 and 100"));
            }

            _logger.LogInformation("Updating battery {BatteryId} charge level to {ChargeLevel}", batteryId, request.ChargeLevel);

            var result = await _inventoryService.UpdateBatteryChargeLevelAsync(batteryId, request.ChargeLevel);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Battery not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Battery charge level updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating battery charge level for {BatteryId}", batteryId);
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; }
}

public class UpdateChargeLevelRequest
{
    public int ChargeLevel { get; set; }
}
