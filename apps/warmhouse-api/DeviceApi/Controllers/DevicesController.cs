using Microsoft.AspNetCore.Mvc;
using DeviceApi.Data;
using DeviceApi.Models;

namespace DeviceApi.Controllers
{
    [Route("api/v2/devices")]
    [ApiController]
    public class DevicesController(DataStore dataStore) : ControllerBase
    {
        private readonly DataStore _dataStore = dataStore;

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "ok" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices([FromQuery] int? houseId, [FromQuery] int? locationId)
        {
            List<Device> devices = await _dataStore.GetDevicesAsync(houseId, locationId);
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDevice(int id)
        {
            Device? device = await _dataStore.GetDeviceAsync(id);
            if (device == null)
                return NotFound("Device not found");

            return Ok(device);
        }

        [HttpPost]
        public IActionResult CreateDevice([FromBody] CreateDeviceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Device device = _dataStore.AddDevice(request);
            return Ok(device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] CreateDeviceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Device? device = await _dataStore.UpdateDeviceAsync(id, request);

            if (device == null)
                return NotFound(new { message = $"Device with ID {id} not found" });

            return Ok(device);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            bool deleted = await _dataStore.DeleteDeviceAsync(id);

            if (!deleted)
                return NotFound("Device not found");

            return Ok(new { message = "Device deleted successfully" });
        }

        [HttpPatch("{id}/value")]
        public async Task<IActionResult> UpdateDeviceValue(int id, [FromBody] UpdateDeviceRequest request)
        {
            Device? device = await _dataStore.UpdateDeviceValueAsync(id, request);

            if (device == null)
                return NotFound(new { message = $"Device with ID {id} not found" });

            return Ok(device);
        }
    }
}
