using MassTransit;
using Shared.Contracts.Messages;

namespace WorkflowEngine.Api.Consumers;

public class NotificationServiceStub : 
    IConsumer<AppointmentNotificationSent>,
    IConsumer<PrescriptionNotificationSent>
{
    private readonly ILogger<NotificationServiceStub> _logger;

    public NotificationServiceStub(ILogger<NotificationServiceStub> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<AppointmentNotificationSent> context)
    {
        _logger.LogInformation("TODO: Integrate with Graph API/SMTP. Would send email/Teams message for Appointment ID {Id}", context.Message.CorrelationId);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<PrescriptionNotificationSent> context)
    {
        _logger.LogInformation("TODO: Integrate with Graph API/SMTP. Would send email/Teams message for Prescription Refill ID {Id}", context.Message.CorrelationId);
        return Task.CompletedTask;
    }
}
