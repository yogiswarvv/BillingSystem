using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedDoctorsCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "Doctor",
                columns: new[] { "DoctorId", "CreatedDate", "FirstName", "IsAvailable", "LastName", "Specialization" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aditya", true, "Verma", "Cardiology" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sriya", true, "Reddy", "Diagnostics" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vikram", true, "Singh", "Radiology" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Doctor",
                keyColumn: "DoctorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Doctor",
                keyColumn: "DoctorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "Doctor",
                keyColumn: "DoctorId",
                keyValue: 3);
        }
    }
}
