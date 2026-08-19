using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class PacsConnector : IHospitalSystemConnector
{
    public string SystemName => "PACS";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    
    public Task<JsonObject?> GetImagingAsync(string patientId)
    {
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("PACS server unavailable.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Studies"] = new JsonArray 
            { 
                new JsonObject { ["StudyDate"] = "2023-08-10", ["Modality"] = "CT", ["Description"] = "CT Abdomen" },
                new JsonObject { ["StudyDate"] = "2022-01-15", ["Modality"] = "XR", ["Description"] = "Chest X-Ray" }
            }
        });
    }
    
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
