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
        [Description("The unique identifier of the patient")] string patientId,
        [Description("The desired date and time for the appointment")] string desiredDate)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { PatientId = patientId, DesiredDate = desiredDate }), Encoding.UTF8, "application/json");
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
        var content = new StringContent(JsonSerializer.Serialize(new { PatientId = patientId, Medication = medication }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://workflowengine-api:8080/workflows/prescription-refill/start", content);
        
        if (!response.IsSuccessStatusCode)
        {
            return $"Error starting refill workflow: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }
}
