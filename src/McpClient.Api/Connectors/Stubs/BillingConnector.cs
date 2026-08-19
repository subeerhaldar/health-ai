using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class BillingConnector : IHospitalSystemConnector
{
    public string SystemName => "Billing";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    
    public Task<JsonObject?> GetBillingInfoAsync(string patientId)
    {
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("Billing gateway timeout.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["OutstandingBalance"] = 150.50,
            ["LastPaymentDate"] = "2023-09-01"
        });
    }
}
