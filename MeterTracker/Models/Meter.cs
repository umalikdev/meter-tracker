using System.Text.Json.Serialization;

namespace MeterTracker.Models;

public class Meter
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("current_reading")]
    public decimal CurrentReading { get; set; }  
    
    [JsonPropertyName("base_reading")]
    public decimal BaseReading { get; set; }

    [JsonPropertyName("previous_reading")]
    public decimal PreviousReading { get; set; }

    [JsonPropertyName("last_updated_by")]
    public string? LastUpdatedBy { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTime? LastUpdatedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}
