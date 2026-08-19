using MassTransit;
using Shared.Contracts.Messages;

namespace WorkflowEngine.Api.Sagas;

public class AppointmentSchedulingSaga : MassTransitStateMachine<AppointmentSagaState>
{
    public State Requested { get; private set; } = null!;
    public State AvailabilityChecked { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State NotificationSent { get; private set; } = null!;

    public Event<StartAppointmentScheduling> StartEvent { get; private set; } = null!;
    public Event<AppointmentAvailabilityChecked> CheckedEvent { get; private set; } = null!;
    public Event<AppointmentConfirmed> ConfirmedEvent { get; private set; } = null!;
    public Event<AppointmentNotificationSent> NotificationEvent { get; private set; } = null!;

    public AppointmentSchedulingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => CheckedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ConfirmedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => NotificationEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(StartEvent)
                .Then(context =>
                {
                    context.Saga.PatientId = context.Message.PatientId;
                    context.Saga.ProviderId = context.Message.ProviderId;
                    context.Saga.RequestedDate = context.Message.RequestedDate;
                })
                .TransitionTo(Requested)
        );

        During(Requested,
            When(CheckedEvent)
                .IfElse(context => context.Message.IsAvailable,
                    binder => binder.TransitionTo(AvailabilityChecked),
                    binder => binder.Finalize()) // Terminate if not available
        );

        During(AvailabilityChecked,
            When(ConfirmedEvent)
                .TransitionTo(Confirmed)
        );

        During(Confirmed,
            When(NotificationEvent)
                .TransitionTo(NotificationSent)
                .Finalize() // Complete the saga
        );

        SetCompletedWhenFinalized();
    }
}
