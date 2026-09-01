using System.Text.Json.Serialization;

namespace MeterTracker.Models;

public class ReadingLog
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("meter_id")]
    public string MeterId { get; set; } = string.Empty;

    [JsonPropertyName("previous_reading")]
    public decimal PreviousReading { get; set; }

    [JsonPropertyName("new_reading")]
    public decimal NewReading { get; set; }

    [JsonPropertyName("units_consumed")]
    public decimal UnitsConsumed { get; set; }

    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

// Used only when POSTing a new meter (no id/timestamps yet).
public class NewMeterRequest
{
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
}

// Used only when POSTing a new log entry (units_consumed is DB-computed, omit it).
public class NewReadingLogRequest
{
    [JsonPropertyName("meter_id")]
    public string MeterId { get; set; } = string.Empty;

    [JsonPropertyName("previous_reading")]
    public decimal PreviousReading { get; set; }

    [JsonPropertyName("new_reading")]
    public decimal NewReading { get; set; }

    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; set; }
}

public class MeterPatchRequest
{
    [JsonPropertyName("current_reading")]
    public decimal CurrentReading { get; set; }

    [JsonPropertyName("previous_reading")]
    public decimal PreviousReading { get; set; }

    [JsonPropertyName("last_updated_by")]
    public string? LastUpdatedBy { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTime LastUpdatedAt { get; set; }
}
