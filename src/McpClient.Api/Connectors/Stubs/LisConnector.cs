using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class LisConnector : IHospitalSystemConnector
{
    public string SystemName => "LIS";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    
    public Task<JsonObject?> GetLabResultsAsync(string patientId)
    {
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("LIS timeout.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Tests"] = new JsonArray 
            { 
                new JsonObject { ["Name"] = "HbA1c", ["Value"] = "6.8%", ["Date"] = "2023-10-01" },
                new JsonObject { ["Name"] = "Cholesterol", ["Value"] = "180 mg/dL", ["Date"] = "2023-10-01" }
            }
        });
    }

    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
