using System.Text.Json.Serialization;

namespace DeviceApi.Models
{
    public class Device
    {
        public int Id { get; set; }

        [JsonPropertyName("serial_number")]
        public string? SerialNumber { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("device_type_id")]
        public int DeviceTypeId { get; set; }

        [JsonPropertyName("type")]
        public string? DeviceType { get; set; }

        [JsonPropertyName("house_id")]
        public int HouseId { get; set; }

        [JsonPropertyName("location_id")]
        public int? LocationId { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; } = "Living Room";

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }
        public string Status { get; set; } = "active";
        public string Metric { get; set; } = "temperature";
        public double? Value { get; set; } = 22.5;
        public string? Unit { get; set; } = "°C";
        public DateTime Timestamp { get; set; }
    }

    public class CreateDeviceRequest
    {
        public required string Name { get; set; }
        public string? Type { get; set; } = "temperature";
        public string Location { get; set; }
        public string Unit { get; set; }

        public string? SerialNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public int? DeviceTypeId { get; set; }
        public int? HouseId { get; set; }
        public int? LocationId { get; set; }
    }

    public class UpdateDeviceRequest
    {
        public double Value { get; set; }
        public string? Status { get; set; } = "active";
    }
}