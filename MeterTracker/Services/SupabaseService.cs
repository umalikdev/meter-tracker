using System.Net.Http.Headers;
using System.Net.Http.Json;
using MeterTracker.Models;

namespace MeterTracker.Services;

/// <summary>
/// Thin wrapper around Supabase's auto-generated REST API (PostgREST).
/// No backend server is involved — the browser calls Supabase directly.
/// </summary>
public class SupabaseService
{
    private readonly HttpClient _http;

    public SupabaseService(HttpClient http)
    {
        _http = http;
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, bool returnRepresentation = false)
    {
        var request = new HttpRequestMessage(method, $"{SupabaseConfig.Url}/rest/v1/{path}");
        request.Headers.Add("apikey", SupabaseConfig.AnonKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SupabaseConfig.AnonKey);
        if (returnRepresentation)
        {
            request.Headers.Add("Prefer", "return=representation");
        }
        return request;
    }

    public async Task<List<Meter>> GetMetersAsync()
    {
        var request = BuildRequest(HttpMethod.Get, "meters?select=*&order=created_at.asc");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Meter>>() ?? new();
    }

    public async Task<Meter?> GetMeterAsync(string id)
    {
        var request = BuildRequest(HttpMethod.Get, $"meters?id=eq.{id}&select=*");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<Meter>>() ?? new();
        return list.FirstOrDefault();
    }

    public async Task<Meter?> AddMeterAsync(string name, decimal startingReading, string? addedBy)
    {
        var request = BuildRequest(HttpMethod.Post, "meters", returnRepresentation: true);
        var payload = new NewMeterRequest
        {
            Name = name,
            BaseReading = startingReading,
            CurrentReading = startingReading, // starts at 0 units consumed
            PreviousReading = 0,
            LastUpdatedBy = addedBy
        };
        request.Content = JsonContent.Create(payload);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<Meter>>() ?? new();
        return list.FirstOrDefault();
    }

    public async Task DeleteMeterAsync(string id)
    {
        var request = BuildRequest(HttpMethod.Delete, $"meters?id=eq.{id}");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ReadingLog>> GetHistoryAsync(string meterId)
    {
        var request = BuildRequest(HttpMethod.Get, $"reading_logs?meter_id=eq.{meterId}&select=*&order=updated_at.desc");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ReadingLog>>() ?? new();
    }

    /// <summary>
    /// Records a new reading: updates the meter's current value AND appends a history row.
    /// </summary>
    public async Task UpdateReadingAsync(Meter meter, decimal newReading, string updatedBy)
    {
        var now = DateTime.UtcNow;

        // 1. Insert the log entry (previous -> new).
        var logRequest = BuildRequest(HttpMethod.Post, "reading_logs");
        logRequest.Content = JsonContent.Create(new NewReadingLogRequest
        {
            MeterId = meter.Id!,
            PreviousReading = meter.CurrentReading,
            NewReading = newReading,
            UpdatedBy = updatedBy
        });
        var logResponse = await _http.SendAsync(logRequest);
        logResponse.EnsureSuccessStatusCode();

        // 2. Update the meter's "current" snapshot fields.
        var patchRequest = BuildRequest(HttpMethod.Patch, $"meters?id=eq.{meter.Id}");
        patchRequest.Content = JsonContent.Create(new MeterPatchRequest
        {
            CurrentReading = newReading,
            PreviousReading = meter.CurrentReading, // what the reading was before this update
            LastUpdatedBy = updatedBy,
            LastUpdatedAt = now
        });
        var patchResponse = await _http.SendAsync(patchRequest);
        patchResponse.EnsureSuccessStatusCode();
    }
}
