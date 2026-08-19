using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors.Stubs;

public class EmrConnector : IHospitalSystemConnector
{
    public string SystemName => "EMR";

    public Task<JsonObject?> GetPatientRecordAsync(string patientId)
    {
        // Simulate a potential failure roughly 10% of the time to test degraded logic
        if (Random.Shared.NextDouble() < 0.1) throw new Exception("EMR connection timed out.");

        return Task.FromResult<JsonObject?>(new JsonObject
        {
            ["System"] = SystemName,
            ["PatientId"] = patientId,
            ["Condition"] = "Type 2 Diabetes",
            ["Medications"] = new JsonArray { "Metformin 500mg", "Lisinopril 10mg" },
            ["LastVisit"] = "2023-10-15"
        });
    }

    public Task<JsonObject?> GetLabResultsAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetImagingAsync(string patientId) => Task.FromResult<JsonObject?>(null);
    public Task<JsonObject?> GetBillingInfoAsync(string patientId) => Task.FromResult<JsonObject?>(null);
}
