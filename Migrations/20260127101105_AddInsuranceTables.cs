using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Healthcare",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Healthcare",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                schema: "Healthcare",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "Healthcare",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "Healthcare",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                schema: "Healthcare",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                schema: "Healthcare",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Healthcare",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ApolloMunich_Registry",
                schema: "Healthcare",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApolloMunich_Registry", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "HDFCErgo_Registry",
                schema: "Healthcare",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDFCErgo_Registry", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                schema: "Healthcare",
                columns: table => new
                {
                    ClaimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillID = table.Column<int>(type: "int", nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClaimAmountRequested = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClaimStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.ClaimID);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Bills_BillID",
                        column: x => x.BillID,
                        principalSchema: "Healthcare",
                        principalTable: "Bills",
                        principalColumn: "BillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceProviders",
                schema: "Healthcare",
                columns: table => new
                {
                    ProviderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsLinked = table.Column<bool>(type: "bit", nullable: false),
                    AgreementExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TollFreeNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceProviders", x => x.ProviderID);
                });

            migrationBuilder.CreateTable(
                name: "StarHealth_Registry",
                schema: "Healthcare",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarHealth_Registry", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePlans",
                schema: "Healthcare",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CoveragePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CoPayAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequiresPreAuth = table.Column<bool>(type: "bit", nullable: false),
                    MaxBenefitPerClaim = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePlans", x => x.PlanID);
                    table.ForeignKey(
                        name: "FK_InsurancePlans_InsuranceProviders_ProviderID",
                        column: x => x.ProviderID,
                        principalSchema: "Healthcare",
                        principalTable: "InsuranceProviders",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceMemberRegistry",
                schema: "Healthcare",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastRenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceMemberRegistry", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_InsuranceMemberRegistry_InsurancePlans_PlanID",
                        column: x => x.PlanID,
                        principalSchema: "Healthcare",
                        principalTable: "InsurancePlans",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsuranceMemberRegistry_InsuranceProviders_ProviderID",
                        column: x => x.ProviderID,
                        principalSchema: "Healthcare",
                        principalTable: "InsuranceProviders",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "InsuranceProviders",
                columns: new[] { "ProviderID", "AgreementExpiryDate", "ContactEmail", "IsLinked", "ProviderName", "TollFreeNumber" },
                values: new object[,]
                {
                    { 1, null, "claims@apollomunich.in", true, "Apollo Munich", "1800-123-4444" },
                    { 2, null, "support@hdfcergo.com", true, "HDFC Ergo", "1800-222-3333" },
                    { 3, null, "info@starhealth.in", true, "Star Health", "1800-555-6666" }
                });

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 1,
                column: "PolicyNumber",
                value: "AM-1001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 2,
                column: "PolicyNumber",
                value: "SH-3001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 3,
                column: "PolicyNumber",
                value: "HE-2001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8164));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8173));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8176));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8179));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8182));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8184));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8187));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8189));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8193));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8195));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8197));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8200));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8202));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8204));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8206));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8209));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8211));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8213));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8216));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 27, 15, 41, 4, 166, DateTimeKind.Local).AddTicks(8218));

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "InsurancePlans",
                columns: new[] { "PlanID", "CoPayAmount", "CoveragePercentage", "MaxBenefitPerClaim", "PlanName", "ProviderID", "RequiresPreAuth" },
                values: new object[,]
                {
                    { 1, 0.00m, 80.00m, 50000m, "Silver Elite", 1, false },
                    { 2, 0.00m, 100.00m, 150000m, "Gold Shield", 1, false },
                    { 3, 0.00m, 90.00m, 80000m, "Easy Health Plus", 2, false },
                    { 4, 0.00m, 100.00m, 500000m, "Optima Premium", 2, false },
                    { 5, 0.00m, 70.00m, 40000m, "Senior Red Carpet", 3, false },
                    { 6, 0.00m, 100.00m, 200000m, "Family Pro", 3, false }
                });

            migrationBuilder.InsertData(
                schema: "Healthcare",
                table: "InsuranceMemberRegistry",
                columns: new[] { "MemberID", "DateOfBirth", "FullName", "Gender", "LastRenewalDate", "PlanID", "PolicyNumber", "ProviderID", "RemainingBalance", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(1985, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aarav Gupta", null, null, 1, "AM-1001", 1, 45000m, "Active" },
                    { 2, new DateTime(1990, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ishani Singh", null, null, 1, "AM-1002", 1, 38000m, "Active" },
                    { 3, new DateTime(1975, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vihaan Sharma", null, null, 2, "AM-1003", 1, 120000m, "Active" },
                    { 4, new DateTime(1988, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Myra Kapoor", null, null, 2, "AM-1004", 1, 95000m, "Active" },
                    { 5, new DateTime(1995, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reyansh Reddy", null, null, 1, "AM-1005", 1, 48000m, "Active" },
                    { 6, new DateTime(1982, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anaya Jain", null, null, 2, "AM-1006", 1, 135000m, "Active" },
                    { 7, new DateTime(1968, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sai Kumar", null, null, 1, "AM-1007", 1, 22000m, "Active" },
                    { 8, new DateTime(1993, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Diya Malhotra", null, null, 2, "AM-1008", 1, 110000m, "Active" },
                    { 9, new DateTime(1970, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advait Bhosale", null, null, 1, "AM-1009", 1, 31000m, "Active" },
                    { 10, new DateTime(1987, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Saanvi Iyer", null, null, 2, "AM-1010", 1, 88000m, "Active" },
                    { 11, new DateTime(1981, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arjun Pillai", null, null, 1, "AM-1011", 1, 42000m, "Active" },
                    { 12, new DateTime(1996, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kavya Nair", null, null, 2, "AM-1012", 1, 145000m, "Active" },
                    { 13, new DateTime(1973, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rudra Pandey", null, null, 1, "AM-1013", 1, 15000m, "Active" },
                    { 14, new DateTime(1984, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anika Gupta", null, null, 2, "AM-1014", 1, 102000m, "Active" },
                    { 15, new DateTime(1991, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vivaan Joshi", null, null, 1, "AM-1015", 1, 49000m, "Active" },
                    { 16, new DateTime(1989, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aira Saxena", null, null, 2, "AM-1016", 1, 121000m, "Active" },
                    { 17, new DateTime(1965, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kabir Mehta", null, null, 1, "AM-1017", 1, 8000m, "Active" },
                    { 18, new DateTime(1998, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vanya Chopra", null, null, 2, "AM-1018", 1, 150000m, "Active" },
                    { 19, new DateTime(1977, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dhruv Agrawal", null, null, 1, "AM-1019", 1, 29000m, "Active" },
                    { 20, new DateTime(1986, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiara Das", null, null, 2, "AM-1020", 1, 115000m, "Active" },
                    { 21, new DateTime(1965, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Suresh Mani", null, null, 3, "HE-2001", 2, 70000m, "Active" },
                    { 22, new DateTime(1992, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ananya Roy", null, null, 3, "HE-2002", 2, 65000m, "Active" },
                    { 23, new DateTime(1980, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rahul Dravid", null, null, 4, "HE-2003", 2, 450000m, "Active" },
                    { 24, new DateTime(1988, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Meera Bai", null, null, 4, "HE-2004", 2, 480000m, "Active" },
                    { 25, new DateTime(1994, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Siddharth Roy", null, null, 3, "HE-2005", 2, 78000m, "Active" },
                    { 26, new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pooja Bhatt", null, null, 4, "HE-2006", 2, 300000m, "Active" },
                    { 27, new DateTime(1955, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amitabh Bach", null, null, 3, "HE-2007", 2, 32000m, "Active" },
                    { 28, new DateTime(1972, 12, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Salman Khan", null, null, 4, "HE-2008", 2, 420000m, "Active" },
                    { 29, new DateTime(1960, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arun Jaitley", null, null, 3, "HE-2009", 2, 55000m, "Active" },
                    { 30, new DateTime(1982, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smriti Irani", null, null, 4, "HE-2010", 2, 350000m, "Active" },
                    { 31, new DateTime(1967, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kapil Dev", null, null, 3, "HE-2011", 2, 68000m, "Active" },
                    { 32, new DateTime(1975, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sunil Gavak", null, null, 4, "HE-2012", 2, 490000m, "Active" },
                    { 33, new DateTime(1983, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Virender Seh", null, null, 3, "HE-2013", 2, 45000m, "Active" },
                    { 34, new DateTime(1981, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sachin Ram", null, null, 4, "HE-2014", 2, 400000m, "Active" },
                    { 35, new DateTime(1990, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rohit Shar", null, null, 3, "HE-2015", 2, 72000m, "Active" },
                    { 36, new DateTime(1987, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shikhar Dhav", null, null, 4, "HE-2016", 2, 380000m, "Active" },
                    { 37, new DateTime(1985, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "M.S. Dhoni", null, null, 3, "HE-2017", 2, 85000m, "Active" },
                    { 38, new DateTime(1986, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yuvraj Singh", null, null, 4, "HE-2018", 2, 450000m, "Active" },
                    { 39, new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jasprit Bum", null, null, 3, "HE-2019", 2, 50000m, "Active" },
                    { 40, new DateTime(1990, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Virat Kohli", null, null, 4, "HE-2020", 2, 500000m, "Active" },
                    { 41, new DateTime(1945, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lata Mangesh", null, null, 5, "SH-3001", 3, 5000m, "Active" },
                    { 42, new DateTime(1955, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mukesh Amb", null, null, 5, "SH-3002", 3, 35000m, "Active" },
                    { 43, new DateTime(1940, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ratan Tata", null, null, 6, "SH-3003", 3, 180000m, "Active" },
                    { 44, new DateTime(1948, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Azim Premji", null, null, 6, "SH-3004", 3, 150000m, "Active" },
                    { 45, new DateTime(1960, 3, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiran Mazum", null, null, 5, "SH-3005", 3, 25000m, "Active" },
                    { 46, new DateTime(1952, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shiv Nadar", null, null, 6, "SH-3006", 3, 190000m, "Active" },
                    { 47, new DateTime(1962, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adani Gautam", null, null, 5, "SH-3007", 3, 38000m, "Active" },
                    { 48, new DateTime(1965, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Uday Kotak", null, null, 6, "SH-3008", 3, 200000m, "Active" },
                    { 49, new DateTime(1958, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Savitri Jind", null, null, 5, "SH-3009", 3, 12000m, "Active" },
                    { 50, new DateTime(1959, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dilip Shangh", null, null, 6, "SH-3010", 3, 175000m, "Active" },
                    { 51, new DateTime(1963, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sunil Mittal", null, null, 5, "SH-3011", 3, 28000m, "Active" },
                    { 52, new DateTime(1967, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kumar Birla", null, null, 6, "SH-3012", 3, 160000m, "Active" },
                    { 53, new DateTime(1950, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cyrus Poonaw", null, null, 5, "SH-3013", 3, 10000m, "Active" },
                    { 54, new DateTime(1954, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Radhakrish", null, null, 6, "SH-3014", 3, 145000m, "Active" },
                    { 55, new DateTime(1961, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pankaj Patel", null, null, 5, "SH-3015", 3, 33000m, "Active" },
                    { 56, new DateTime(1987, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nikhil Kamat", null, null, 6, "SH-3016", 3, 195000m, "Active" },
                    { 57, new DateTime(1973, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vijay Sharma", null, null, 5, "SH-3017", 3, 21000m, "Active" },
                    { 58, new DateTime(1984, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Deepinder G", null, null, 6, "SH-3018", 3, 130000m, "Active" },
                    { 59, new DateTime(1986, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bhavish Agg", null, null, 5, "SH-3019", 3, 19000m, "Active" },
                    { 60, new DateTime(1983, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kunal Shah", null, null, 6, "SH-3020", 3, 120000m, "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_BillID",
                schema: "Healthcare",
                table: "InsuranceClaims",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceMemberRegistry_PlanID",
                schema: "Healthcare",
                table: "InsuranceMemberRegistry",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceMemberRegistry_ProviderID",
                schema: "Healthcare",
                table: "InsuranceMemberRegistry",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePlans_ProviderID",
                schema: "Healthcare",
                table: "InsurancePlans",
                column: "ProviderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApolloMunich_Registry",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "HDFCErgo_Registry",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "InsuranceClaims",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "InsuranceMemberRegistry",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "StarHealth_Registry",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "InsurancePlans",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "InsuranceProviders",
                schema: "Healthcare");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "Healthcare",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                schema: "Healthcare",
                table: "AspNetRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Healthcare",
                table: "AspNetRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 1,
                column: "PolicyNumber",
                value: "POL12345");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 2,
                column: "PolicyNumber",
                value: "STAR999");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Insurances",
                keyColumn: "InsuranceId",
                keyValue: 3,
                column: "PolicyNumber",
                value: "HDFC001");

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5754));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5763));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5766));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5768));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5771));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5773));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5775));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5777));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5780));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5782));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5784));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5787));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5789));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5792));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5794));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5796));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5798));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5801));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5803));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 18, 0, 58, 206, DateTimeKind.Local).AddTicks(5805));

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Healthcare",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Healthcare",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "Healthcare",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Healthcare",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Healthcare",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalSchema: "Healthcare",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Healthcare",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "Healthcare",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
