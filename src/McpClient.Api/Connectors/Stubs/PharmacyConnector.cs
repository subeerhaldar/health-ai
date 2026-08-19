using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class PharmacyConnector : IHospitalSystemConnector
{
    public string SystemName => "Pharmacy";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId)
    {
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("Pharmacy DB unreachable.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["ActivePrescriptions"] = new JsonArray 
            { 
                new JsonObject { ["Drug"] = "Lisinopril 10mg", ["RefillsRemaining"] = 2 }
            }
        });
    }

    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
