using System.Text.Json.Serialization;

namespace DeviceApi.Models
{
    public class TelemetryResponse
    {
        public int Id { get; set; }
        [JsonPropertyName("sensor_id")]
        public int DeviceId { get; set; }
        public string Status { get; set; } = "active";
        public string Metric { get; set; } = "temperature";
        public double Value { get; set; }
        public string Unit { get; set; } = "°C";
        public DateTime Timestamp { get; set; }
    }
}