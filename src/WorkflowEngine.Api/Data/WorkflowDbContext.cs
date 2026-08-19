using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowEngine.Api.Sagas;

namespace WorkflowEngine.Api.Data;

public class WorkflowDbContext : SagaDbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new AppointmentSagaMap();
            yield return new PrescriptionSagaMap();
        }
    }
}

public class AppointmentSagaMap : SagaClassMap<AppointmentSagaState>
{
    protected override void Configure(EntityTypeBuilder<AppointmentSagaState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.PatientId).HasMaxLength(128);
        entity.Property(x => x.ProviderId).HasMaxLength(128);
    }
}

public class PrescriptionSagaMap : SagaClassMap<PrescriptionSagaState>
{
    protected override void Configure(EntityTypeBuilder<PrescriptionSagaState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.PatientId).HasMaxLength(128);
        entity.Property(x => x.MedicationId).HasMaxLength(128);
        entity.Property(x => x.DenialReason).HasMaxLength(256);
    }
}
