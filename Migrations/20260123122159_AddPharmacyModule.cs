using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicines",
                schema: "Healthcare",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DosageStrength = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineId);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                schema: "Healthcare",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    SuggestedQuantity = table.Column<int>(type: "int", nullable: false),
                    ActualQuantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PrescribedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "Healthcare",
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalSchema: "Healthcare",
                        principalTable: "Medicines",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "Medicines",
                columns: new[] { "MedicineId", "Category", "CreatedDate", "DosageStrength", "ExpiryDate", "IsActive", "Name", "PricePerUnit", "Stock" },
                values: new object[,]
                {
                    { 1, "Analgesic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1986), "500mg", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Paracetamol", 2m, 1000 },
                    { 2, "Antibiotic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1994), "250mg", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Amoxicillin", 15m, 500 },
                    { 3, "Antihistamine", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(1997), "10mg", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Cetirizine", 5m, 800 },
                    { 4, "Antidiabetic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2000), "500mg", new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Metformin", 8m, 1200 },
                    { 5, "Statin", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2003), "20mg", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Atorvastatin", 25m, 300 },
                    { 6, "Proton Pump Inhibitor", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2006), "20mg", new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Omeprazole", 12m, 500 },
                    { 7, "Bloop Pressure", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2008), "5mg", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Amlodipine", 10m, 600 },
                    { 8, "Analgesic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2010), "400mg", new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ibuprofen", 3m, 900 },
                    { 9, "Antibiotic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2012), "500mg", new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Azithromycin", 45m, 200 },
                    { 10, "Beta Blocker", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2015), "500mg", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Metoprolol", 18m, 400 },
                    { 11, "Antihypertensive", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2017), "500mg", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Losartan", 22m, 350 },
                    { 12, "Anticonvulsant", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2019), "300mg", new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Gabapentin", 35m, 250 },
                    { 13, "Antidepressant", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2022), "50mg", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Sertraline", 40m, 150 },
                    { 14, "Proton Pump Inhibitor", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2026), "40mg", new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Pantoprazole", 14m, 500 },
                    { 15, "Antiplatelet", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2029), "75mg", new DateTime(2027, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Aspirin", 1m, 2000 },
                    { 16, "Corticosteroid", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2032), "5mg", new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Prednisone", 20m, 300 },
                    { 17, "Bronchodilator", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2034), "100mcg", new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Salbutamol", 150m, 100 },
                    { 18, "Diuretic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2036), "40mg", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Furosemide", 6m, 500 },
                    { 19, "Insulin", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2038), "100U/ml", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Insulin Glargine", 800m, 50 },
                    { 20, "Antibiotic", new DateTime(2026, 1, 23, 17, 51, 58, 701, DateTimeKind.Local).AddTicks(2040), "500mg", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ciprofloxacin", 30m, 400 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_AppointmentId",
                schema: "Healthcare",
                table: "Prescriptions",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicineId",
                schema: "Healthcare",
                table: "Prescriptions",
                column: "MedicineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prescriptions",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "Medicines",
                schema: "Healthcare");
        }
    }
}
