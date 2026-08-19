using MassTransit;

namespace Shared.Contracts.Messages;

// Events for Appointment Scheduling Saga

public interface StartAppointmentScheduling : CorrelatedBy<Guid>
{
    string PatientId { get; }
    string ProviderId { get; }
    DateTime RequestedDate { get; }
}

public interface AppointmentAvailabilityChecked : CorrelatedBy<Guid>
{
    bool IsAvailable { get; }
}

public interface AppointmentConfirmed : CorrelatedBy<Guid>
{
}

public interface AppointmentNotificationSent : CorrelatedBy<Guid>
{
}
