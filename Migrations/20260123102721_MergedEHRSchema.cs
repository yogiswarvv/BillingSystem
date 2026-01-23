using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class MergedEHRSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                schema: "Healthcare",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Age",
                schema: "Healthcare",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsSenior",
                schema: "Healthcare",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "Healthcare",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ActionDate",
                schema: "Healthcare",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityName",
                schema: "Healthcare",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Healthcare",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "Healthcare",
                newName: "AuditLog",
                newSchema: "Healthcare");

            migrationBuilder.RenameColumn(
                name: "FullName",
                schema: "Healthcare",
                table: "Patients",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "Healthcare",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                schema: "Healthcare",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Healthcare",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChangedBy",
                schema: "Healthcare",
                table: "AuditLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "SYSTEM");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedDate",
                schema: "Healthcare",
                table: "AuditLog",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Operation",
                schema: "Healthcare",
                table: "AuditLog",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                schema: "Healthcare",
                table: "AuditLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                schema: "Healthcare",
                table: "AuditLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLog",
                schema: "Healthcare",
                table: "AuditLog",
                column: "AuditLogId");

            migrationBuilder.CreateTable(
                name: "Doctor",
                schema: "Healthcare",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.DoctorId);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                schema: "Healthcare",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DoctorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DoctorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointment_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "Healthcare",
                        principalTable: "Doctor",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "Healthcare",
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOrder",
                schema: "Healthcare",
                columns: table => new
                {
                    LabOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    Results = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrder", x => x.LabOrderId);
                    table.ForeignKey(
                        name: "FK_LabOrder_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "Healthcare",
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId",
                schema: "Healthcare",
                table: "Appointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                schema: "Healthcare",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_AppointmentId",
                schema: "Healthcare",
                table: "LabOrder",
                column: "AppointmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabOrder",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "Appointment",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "Doctor",
                schema: "Healthcare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLog",
                schema: "Healthcare",
                table: "AuditLog");


            migrationBuilder.DropColumn(
                name: "Address",
                schema: "Healthcare",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "Healthcare",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Healthcare",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                schema: "Healthcare",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "ChangedDate",
                schema: "Healthcare",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "Operation",
                schema: "Healthcare",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "RecordId",
                schema: "Healthcare",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "TableName",
                schema: "Healthcare",
                table: "AuditLog");

            migrationBuilder.RenameTable(
                name: "AuditLog",
                schema: "Healthcare",
                newName: "AuditLogs",
                newSchema: "Healthcare");

            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "Healthcare",
                table: "Patients",
                newName: "FullName");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                schema: "Healthcare",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSenior",
                schema: "Healthcare",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                schema: "Healthcare",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionDate",
                schema: "Healthcare",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                schema: "Healthcare",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Healthcare",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                schema: "Healthcare",
                table: "AuditLogs",
                column: "AuditLogId");
        }
    }
}
