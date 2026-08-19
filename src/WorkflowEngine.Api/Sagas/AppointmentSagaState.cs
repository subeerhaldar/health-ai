using MassTransit;

namespace WorkflowEngine.Api.Sagas;

public class AppointmentSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    
    public string CurrentState { get; set; } = string.Empty;
    
    public string? PatientId { get; set; }
    public string? ProviderId { get; set; }
    public DateTime? RequestedDate { get; set; }
}
