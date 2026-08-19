using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class HisConnector : IHospitalSystemConnector
{
    public string SystemName => "HIS";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId)
    {
        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Admissions"] = new JsonArray 
            { 
                new JsonObject { ["Date"] = "2023-10-15", ["Department"] = "Endocrinology", ["Status"] = "Discharged" }
            }
        });
    }

    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
