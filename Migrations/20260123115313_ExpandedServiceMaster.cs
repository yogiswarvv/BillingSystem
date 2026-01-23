using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedServiceMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 1,
                column: "ServiceCode",
                value: "LAB001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 2,
                column: "ServiceCode",
                value: "RAD001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 3,
                column: "ServiceCode",
                value: "CRD001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 4,
                column: "ServiceCode",
                value: "RAD002");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 5,
                column: "ServiceCode",
                value: "RAD003");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 6,
                column: "ServiceCode",
                value: "LAB002");

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "ServicesMaster",
                columns: new[] { "ServiceId", "Cost", "CreatedDate", "Department", "IsActive", "ServiceCode", "ServiceName" },
                values: new object[,]
                {
                    { 7, 800m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pathology", true, "LAB007", "Lipid Profile" },
                    { 8, 950m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pathology", true, "LAB008", "Kidney Function Test" },
                    { 9, 1100m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pathology", true, "LAB009", "Thyroid Profile" },
                    { 10, 100m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pathology", true, "LAB010", "Blood Sugar" },
                    { 11, 1200m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pathology", true, "LAB011", "Liver Function Test" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 11);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 1,
                column: "ServiceCode",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 2,
                column: "ServiceCode",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 3,
                column: "ServiceCode",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 4,
                column: "ServiceCode",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 5,
                column: "ServiceCode",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "ServicesMaster",
                keyColumn: "ServiceId",
                keyValue: 6,
                column: "ServiceCode",
                value: null);
        }
    }
}
