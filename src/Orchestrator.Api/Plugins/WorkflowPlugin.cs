using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Orchestrator.Api.Plugins;

public class WorkflowPlugin
{
    private readonly HttpClient _httpClient;

    public WorkflowPlugin(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [KernelFunction, Description("Starts a new workflow to schedule an appointment for a patient.")]
    public async Task<string> ScheduleAppointmentAsync(
        [Description("The unique identifier of the patient (e.g. p-123 or JohnDoe123)")] string patientId,
        [Description("The provider or doctor ID/name (optional, default 'Dr. Smith')")] string? providerId,
        [Description("The requested appointment date and time in ISO 8601 format (e.g., 2026-08-31T09:00:00Z) or YYYY-MM-DD format")] string requestedDate)
    {
        if (!DateTime.TryParse(requestedDate, out var parsedDate))
        {
            parsedDate = DateTime.UtcNow.AddDays(7);
        }
        else
        {
            parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
        }

        var payload = new
        {
            PatientId = patientId,
            ProviderId = string.IsNullOrWhiteSpace(providerId) ? "Dr. Smith" : providerId,
            RequestedDate = parsedDate
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://workflowengine-api:8080/workflows/appointment/start", content);
        
        if (!response.IsSuccessStatusCode)
        {
            return $"Error starting appointment workflow: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }

    [KernelFunction, Description("Starts a new workflow to request a prescription refill for a patient.")]
    public async Task<string> RequestPrescriptionRefillAsync(
        [Description("The unique identifier of the patient")] string patientId,
        [Description("The name of the medication to refill")] string medication)
    {
        var payload = new
        {
            PatientId = patientId,
            MedicationId = medication
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://workflowengine-api:8080/workflows/prescription-refill/start", content);
        
        if (!response.IsSuccessStatusCode)
        {
            return $"Error starting refill workflow: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }
}
