using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Messages;
using WorkflowEngine.Api.Sagas;
using Xunit;

namespace WorkflowEngine.Tests;

public class AppointmentSagaTests
{
    [Fact]
    public async Task Saga_Should_Transition_To_Requested_On_Start()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<AppointmentSchedulingSaga, AppointmentSagaState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var correlationId = Guid.NewGuid();

        // Act
        await harness.Bus.Publish<StartAppointmentScheduling>(new
        {
            CorrelationId = correlationId,
            PatientId = "p-123",
            ProviderId = "dr-456",
            RequestedDate = DateTime.UtcNow
        });

        // Assert
        Assert.True(await harness.Published.Any<StartAppointmentScheduling>());
        
        var sagaHarness = harness.GetSagaStateMachineHarness<AppointmentSchedulingSaga, AppointmentSagaState>();
        Assert.True(await sagaHarness.Consumed.Any<StartAppointmentScheduling>());
        
        var instance = sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Requested);
        Assert.NotNull(instance);
        Assert.Equal("p-123", instance.PatientId);
    }
}
