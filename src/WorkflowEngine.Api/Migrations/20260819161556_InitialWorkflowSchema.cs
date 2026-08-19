using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialWorkflowSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentSagaState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PatientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ProviderId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSagaState", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionSagaState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PatientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MedicationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DenialReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionSagaState", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentSagaState");

            migrationBuilder.DropTable(
                name: "PrescriptionSagaState");
        }
    }
}
