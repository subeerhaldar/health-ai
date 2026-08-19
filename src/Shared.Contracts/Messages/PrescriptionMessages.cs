using MassTransit;

namespace Shared.Contracts.Messages;

// Events for Prescription Refill Saga

public interface StartPrescriptionRefill : CorrelatedBy<Guid>
{
    string PatientId { get; }
    string MedicationId { get; }
}

public interface PrescriptionEligibilityChecked : CorrelatedBy<Guid>
{
    bool IsEligible { get; }
}

public interface PrescriptionApproved : CorrelatedBy<Guid>
{
}

public interface PrescriptionDenied : CorrelatedBy<Guid>
{
    string Reason { get; }
}

public interface PrescriptionNotificationSent : CorrelatedBy<Guid>
{
}
