using System.Text.Json.Nodes;

namespace McpClient.Api.Connectors;

public interface IHospitalSystemConnector
{
    string SystemName { get; }
    
    Task<JsonObject?> GetPatientRecordAsync(string patientId);
    Task<JsonObject?> GetLabResultsAsync(string patientId);
    Task<JsonObject?> GetImagingAsync(string patientId);
    Task<JsonObject?> GetBillingInfoAsync(string patientId);
}
