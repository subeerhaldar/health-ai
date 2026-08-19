using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Messages;
using WorkflowEngine.Api.Sagas;
using Xunit;

namespace WorkflowEngine.Tests;

public class PrescriptionSagaTests
{
    [Fact]
    public async Task Saga_Should_Transition_To_Approved_When_Eligible_And_Approved()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<PrescriptionRefillSaga, PrescriptionSagaState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<PrescriptionRefillSaga, PrescriptionSagaState>();

        var correlationId = Guid.NewGuid();

        // Act 1: Start
        await harness.Bus.Publish<StartPrescriptionRefill>(new
        {
            CorrelationId = correlationId,
            PatientId = "p-999",
            MedicationId = "med-001"
        });

        Assert.True(await sagaHarness.Consumed.Any<StartPrescriptionRefill>());
        Assert.NotNull(sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Requested));

        // Act 2: Eligibility Checked
        await harness.Bus.Publish<PrescriptionEligibilityChecked>(new
        {
            CorrelationId = correlationId,
            IsEligible = true
        });

        Assert.True(await sagaHarness.Consumed.Any<PrescriptionEligibilityChecked>());
        Assert.NotNull(sagaHarness.Sagas.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.ApprovalPending));

        // Act 3: Approved
        await harness.Bus.Publish<PrescriptionApproved>(new
        {
            CorrelationId = correlationId
        });

        Assert.True(await sagaHarness.Consumed.Any<PrescriptionApproved>());
        Assert.NotNull(sagaHarness.Sagas.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Approved));
    }
}
