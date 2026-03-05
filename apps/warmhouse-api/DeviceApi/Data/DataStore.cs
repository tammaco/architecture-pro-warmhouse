using DeviceApi.Models;
using System.Data;
using System.Text.Json;

namespace DeviceApi.Data
{
    public class DataStore
    {
        private readonly List<Device> _devices = [];
        private int _nextDeviceId = 1;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DataStore> _logger;


        public DataStore(IConfiguration configuration, ILogger<DataStore> logger)
        {
            var telemetryApiUrl = configuration["TelemetryApi:BaseUrl"] ?? "http://localhost:5001";
            _logger = logger;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(telemetryApiUrl)
            };

            _logger.LogInformation($"DataStore initialized with Telemetry API URL: {telemetryApiUrl}");

            _devices = new List<Device>
            {
                new Device {
                    Id = _nextDeviceId++,
                    SerialNumber = "SN123456",
                    Name = "Living Room Thermostat",
                    IsActive = true,
                    DeviceTypeId = 1,
                    DeviceType = "Thermostat",
                    HouseId = 1,
                    LocationId = 1,
                    Location = "Living Room",
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now.AddHours(-1),
                },
                new Device {
                    Id = _nextDeviceId++,
                    SerialNumber = "SN789012",
                    Name = "Bedroom Sensor",
                    IsActive = true,
                    DeviceTypeId = 2,
                    DeviceType = "Sensor",
                    HouseId = 1,
                    LocationId = 2,
                    Location = "Bedroom",
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now.AddHours(-1),
                }
            };
        }

        public async Task<List<Device>> GetDevicesAsync(int? houseId = null, int? locationId = null)
        {
            var query = _devices.AsEnumerable();

            if (houseId.HasValue)
                query = query.Where(d => d.HouseId == houseId);

            if (locationId.HasValue)
                query = query.Where(d => d.LocationId == locationId);

            List<Device> devices = [.. query];

            var tasks = devices.Select(async device =>
            {
                var telemetry = await GetTelemetryDataAsync(device.Id);
                SetTelemetryData(telemetry, ref device);
            });

            await Task.WhenAll(tasks);

            return devices;
        }

        private async Task<TelemetryResponse?> GetTelemetryDataAsync(int deviceId)
        {
            var response = await _httpClient.GetAsync($"/api/v2/telemetry/{deviceId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<TelemetryResponse>(jsonResponse, options);
            }

            return null;
        }

        private static void SetTelemetryData(TelemetryResponse? telemetry, ref Device device)
        {
            if (telemetry is not null)
            {
                device.Status = telemetry.Status;
                device.Metric = telemetry.Metric;
                device.Unit = telemetry.Unit;
                device.Value = telemetry.Value;
                device.Timestamp = telemetry.Timestamp;
            }
            else
                device.Timestamp = DateTime.Now;
        }

        public async Task<Device?> GetDeviceAsync(int id)
        {
            Device? device = _devices.FirstOrDefault(d => d.Id == id);
            if (device is not null)
            {
                TelemetryResponse? telemetry = await GetTelemetryDataAsync(id);
                SetTelemetryData(telemetry, ref device);
            }

            return device;
        }

        public Device AddDevice(CreateDeviceRequest request)
        {
            var device = new Device
            {
                Id = _nextDeviceId++,
                Name = request.Name,

                SerialNumber = request.SerialNumber,
                IsActive = request.IsActive,
                DeviceTypeId = request.DeviceTypeId ?? 1,
                HouseId = request.HouseId ?? 1,
                LocationId = request.LocationId ?? 1,
                CreatedAt = DateTime.Now,
            };

            _devices.Add(device);
            return device;
        }

        public async Task<Device?> UpdateDeviceAsync(int id, CreateDeviceRequest request)
        {
            var device = await GetDeviceAsync(id);
            if (device == null) return null;

            device.SerialNumber = request.SerialNumber ?? $"SerialNumber {id}";
            device.Name = request.Name;
            device.IsActive = request.IsActive;
            device.DeviceTypeId = request.DeviceTypeId ?? 1;
            device.HouseId = request.HouseId ?? 1;
            device.LocationId = request.LocationId ?? 1;

            return device;
        }

        public async Task<Device?> UpdateDeviceValueAsync(int id, UpdateDeviceRequest request)
        {
            var device = await GetDeviceAsync(id);
            if (device == null) return null;

            device.Value = request.Value;
            device.IsActive = request.Status == "active" ? true : false;

            return device;
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var device = await GetDeviceAsync(id);
            if (device == null) return false;

            _devices.Remove(device);
            return true;
        }
    }
}