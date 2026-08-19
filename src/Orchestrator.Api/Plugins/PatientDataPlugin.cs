using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Orchestrator.Api.Plugins;

public class PatientDataPlugin
{
    private readonly HttpClient _httpClient;

    public PatientDataPlugin(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [KernelFunction, Description("Gets the general patient record and demographics for a given patient ID.")]
    public async Task<string> GetPatientRecordAsync([Description("The unique identifier of the patient")] string patientId)
    {
        var response = await _httpClient.GetAsync($"http://mcpclient-api:8080/mcp/patient-records/{patientId}");
        if (!response.IsSuccessStatusCode)
        {
            return $"Error fetching patient record: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }

    [KernelFunction, Description("Gets the recent lab results for a given patient ID.")]
    public async Task<string> GetLabResultsAsync([Description("The unique identifier of the patient")] string patientId)
    {
        var response = await _httpClient.GetAsync($"http://mcpclient-api:8080/mcp/lab-results/{patientId}");
        if (!response.IsSuccessStatusCode)
        {
            return $"Error fetching lab results: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }
}
