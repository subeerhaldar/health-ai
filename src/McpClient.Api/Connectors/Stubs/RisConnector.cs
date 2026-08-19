using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class RisConnector : IHospitalSystemConnector
{
    public string SystemName => "RIS";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    
    public Task<JsonObject?> GetImagingAsync(string patientId)
    {
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("RIS connection failed.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Reports"] = new JsonArray 
            { 
                new JsonObject { ["StudyDate"] = "2023-08-10", ["Findings"] = "No acute findings in abdomen." }
            }
        });
    }
    
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
