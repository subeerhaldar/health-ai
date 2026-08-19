using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class CrmConnector : IHospitalSystemConnector
{
    public string SystemName => "CRM";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId)
    {
        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Preferences"] = new JsonObject
            {
                ["ContactMethod"] = "Email",
                ["Language"] = "English"
            }
        });
    }

    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
