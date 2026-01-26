using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientArchivalSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PastRecordPatients",
                schema: "Healthcare",
                columns: table => new
                {
                    PastPatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalPatientId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastRecordPatients", x => x.PastPatientId);
                });

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6226));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6233));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6236));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6238));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6242));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6244));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6246));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6248));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6250));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6253));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6255));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6257));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6259));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6261));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6263));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6266));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6269));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6271));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6273));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 16, 25, 41, 981, DateTimeKind.Local).AddTicks(6275));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PastRecordPatients",
                schema: "Healthcare");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5928));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5937));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5940));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5942));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5947));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5949));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5951));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5954));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5957));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5959));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5961));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5964));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5966));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5968));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5970));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5972));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5975));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5977));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5980));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 25, 13, 20, 34, 273, DateTimeKind.Local).AddTicks(5982));
        }
    }
}
