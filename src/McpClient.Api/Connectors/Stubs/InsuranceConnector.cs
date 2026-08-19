using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class InsuranceConnector : IHospitalSystemConnector
{
    public string SystemName => "Insurance";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    
    public Task<JsonObject?> GetBillingInfoAsync(string patientId)
    {
        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Provider"] = "Blue Cross",
            ["Status"] = "Active",
            ["Copay"] = 25.00
        });
    }
}
