using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using McpClient.Api.Connectors;
using McpClient.Api.Connectors.Stubs;
using McpClient.Api.Models;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

// Register Connectors
builder.Services.AddScoped<IHospitalSystemConnector, EmrConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, PacsConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, LisConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, RisConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, HisConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, PharmacyConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, InsuranceConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, BillingConnector>();
builder.Services.AddScoped<IHospitalSystemConnector, CrmConnector>();

builder.Services.AddScoped<IHospitalSystemConnectorFactory, HospitalSystemConnectorFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");

// Helper method for aggregating data across multiple connectors gracefully
async Task<AggregatedResponse<List<JsonObject>>> AggregateDataAsync(
    IHospitalSystemConnectorFactory factory,
    string[] systems,
    Func<IHospitalSystemConnector, Task<JsonObject?>> fetchAction)
{
    var response = new AggregatedResponse<List<JsonObject>>(new List<JsonObject>());
    
    foreach (var sys in systems)
    {
        try
        {
            var connector = factory.GetConnector(sys);
            var result = await fetchAction(connector);
            if (result != null)
            {
                response.Data!.Add(result);
            }
        }
        catch (Exception ex)
        {
            response.Errors.Add($"Error fetching from {sys}: {ex.Message}");
        }
    }
    return response;
}

// Endpoints
app.MapGet("/mcp/patient-records/{patientId}", async (string patientId, IHospitalSystemConnectorFactory factory) =>
{
    var systems = new[] { "EMR", "HIS", "CRM", "Pharmacy" };
    var result = await AggregateDataAsync(factory, systems, c => c.GetPatientRecordAsync(patientId));
    return Results.Content(System.Text.Json.JsonSerializer.Serialize(result), "application/json");
});

app.MapGet("/mcp/lab-results/{patientId}", async (string patientId, IHospitalSystemConnectorFactory factory) =>
{
    var systems = new[] { "LIS" };
    var result = await AggregateDataAsync(factory, systems, c => c.GetLabResultsAsync(patientId));
    return Results.Content(System.Text.Json.JsonSerializer.Serialize(result), "application/json");
});

app.MapGet("/mcp/imaging/{patientId}", async (string patientId, IHospitalSystemConnectorFactory factory) =>
{
    var systems = new[] { "PACS", "RIS" };
    var result = await AggregateDataAsync(factory, systems, c => c.GetImagingAsync(patientId));
    return Results.Content(System.Text.Json.JsonSerializer.Serialize(result), "application/json");
});

app.MapGet("/mcp/billing/{patientId}", async (string patientId, IHospitalSystemConnectorFactory factory) =>
{
    var systems = new[] { "Billing", "Insurance" };
    var result = await AggregateDataAsync(factory, systems, c => c.GetBillingInfoAsync(patientId));
    return Results.Content(System.Text.Json.JsonSerializer.Serialize(result), "application/json");
});

app.MapGet("/", () => "MCP Client API is running!");

app.Run();

// Make Program public for integration testing
public partial class Program { }
