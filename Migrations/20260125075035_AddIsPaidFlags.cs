using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPaidFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                schema: "Healthcare",
                table: "Appointment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                schema: "Healthcare",
                table: "Admissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "Patients",
                columns: new[] { "PatientId", "CreatedDate", "DateOfBirth", "FirstName", "Gender", "IsActive", "LastName", "MobileNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", 1, true, "Doe", "9876543210" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane", 2, true, "Smith", "8765432109" }
                });

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "Insurances",
                columns: new[] { "InsuranceId", "CoveragePercent", "CoverageType", "IsActive", "PatientId", "PolicyNumber", "ProviderName" },
                values: new object[,]
                {
                    { 1, 80.0, 1, true, 1, "POL12345", "Apollo Munich" },
                    { 2, 50.0, 2, true, 1, "STAR999", "Star Health" },
                    { 3, 100.0, 3, true, 2, "HDFC001", "HDFC Ergo" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IsPaid",
                schema: "Healthcare",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                schema: "Healthcare",
                table: "Admissions");

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1986));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1994));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1997));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2000));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2003));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2006));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2008));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2010));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2012));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2015));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2017));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2019));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2022));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2026));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2029));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2032));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2034));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2036));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2038));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2040));
        }
    }
}
