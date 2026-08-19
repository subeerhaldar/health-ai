using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Shared.Contracts.Messages;
using Shared.Infrastructure.RabbitMq;
using WorkflowEngine.Api.Consumers;
using WorkflowEngine.Api.Data;
using WorkflowEngine.Api.Sagas;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

// Configure DbContext for Saga State
builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure MassTransit
builder.Services.AddRabbitMqMessaging(builder.Configuration, x =>
{
    // Register Notification Consumer
    x.AddConsumer<NotificationServiceStub>();

    // Register Appointment Saga
    x.AddSagaStateMachine<AppointmentSchedulingSaga, AppointmentSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<WorkflowDbContext>();
            r.UsePostgres();
        });

    // Register Prescription Saga
    x.AddSagaStateMachine<PrescriptionRefillSaga, PrescriptionSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<WorkflowDbContext>();
            r.UsePostgres();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Run Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");

// API Endpoints

app.MapPost("/workflows/appointment/start", async (StartAppointmentRequest request, IPublishEndpoint publishEndpoint) =>
{
    var correlationId = Guid.NewGuid();
    await publishEndpoint.Publish<StartAppointmentScheduling>(new
    {
        CorrelationId = correlationId,
        PatientId = request.PatientId,
        ProviderId = request.ProviderId,
        RequestedDate = request.RequestedDate
    });
    return Results.Accepted($"/workflows/{correlationId}/status", new { WorkflowId = correlationId });
});

app.MapPost("/workflows/prescription-refill/start", async (StartPrescriptionRequest request, IPublishEndpoint publishEndpoint) =>
{
    var correlationId = Guid.NewGuid();
    await publishEndpoint.Publish<StartPrescriptionRefill>(new
    {
        CorrelationId = correlationId,
        PatientId = request.PatientId,
        MedicationId = request.MedicationId
    });
    return Results.Accepted($"/workflows/{correlationId}/status", new { WorkflowId = correlationId });
});

app.MapGet("/workflows/{workflowId}/status", async (Guid workflowId, WorkflowDbContext dbContext) =>
{
    var appointmentSaga = await dbContext.Set<AppointmentSagaState>().FindAsync(workflowId);
    if (appointmentSaga != null)
    {
        return Results.Ok(new { WorkflowType = "Appointment", State = appointmentSaga.CurrentState });
    }

    var prescriptionSaga = await dbContext.Set<PrescriptionSagaState>().FindAsync(workflowId);
    if (prescriptionSaga != null)
    {
        return Results.Ok(new { WorkflowType = "PrescriptionRefill", State = prescriptionSaga.CurrentState });
    }

    return Results.NotFound(new { Message = "Workflow not found" });
});

app.MapGet("/", () => "Workflow Engine API is running!");

app.Run();

// Request Models
public record StartAppointmentRequest(string PatientId, string ProviderId, DateTime RequestedDate);
public record StartPrescriptionRequest(string PatientId, string MedicationId);

// Expose for testing
public partial class Program { }
