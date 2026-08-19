using MassTransit;
using Shared.Contracts.Messages;

namespace WorkflowEngine.Api.Sagas;

public class PrescriptionRefillSaga : MassTransitStateMachine<PrescriptionSagaState>
{
    public State Requested { get; private set; } = null!;
    public State EligibilityChecked { get; private set; } = null!;
    public State ApprovalPending { get; private set; } = null!;
    public State Approved { get; private set; } = null!;
    public State Denied { get; private set; } = null!;
    public State NotificationSent { get; private set; } = null!;

    public Event<StartPrescriptionRefill> StartEvent { get; private set; } = null!;
    public Event<PrescriptionEligibilityChecked> CheckedEvent { get; private set; } = null!;
    public Event<PrescriptionApproved> ApprovedEvent { get; private set; } = null!;
    public Event<PrescriptionDenied> DeniedEvent { get; private set; } = null!;
    public Event<PrescriptionNotificationSent> NotificationEvent { get; private set; } = null!;

    public PrescriptionRefillSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => CheckedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ApprovedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => DeniedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => NotificationEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(StartEvent)
                .Then(context =>
                {
                    context.Saga.PatientId = context.Message.PatientId;
                    context.Saga.MedicationId = context.Message.MedicationId;
                })
                .TransitionTo(Requested)
        );

        During(Requested,
            When(CheckedEvent)
                .IfElse(context => context.Message.IsEligible,
                    binder => binder.TransitionTo(ApprovalPending),
                    binder => binder.Then(c => c.Saga.DenialReason = "Ineligible").TransitionTo(Denied))
        );

        During(ApprovalPending,
            When(ApprovedEvent)
                .TransitionTo(Approved),
            When(DeniedEvent)
                .Then(context => context.Saga.DenialReason = context.Message.Reason)
                .TransitionTo(Denied)
        );

        During(Approved, Denied,
            When(NotificationEvent)
                .TransitionTo(NotificationSent)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
