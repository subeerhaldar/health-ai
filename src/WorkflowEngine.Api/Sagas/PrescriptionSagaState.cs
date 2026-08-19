using MassTransit;

namespace WorkflowEngine.Api.Sagas;

public class PrescriptionSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    
    public string CurrentState { get; set; } = string.Empty;
    
    public string? PatientId { get; set; }
    public string? MedicationId { get; set; }
    public string? DenialReason { get; set; }
}
