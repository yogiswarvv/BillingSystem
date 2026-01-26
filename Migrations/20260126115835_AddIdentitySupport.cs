using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                schema: "Healthcare",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Patients_PatientId",
                schema: "Healthcare",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK_Insurances_Patients_PatientId",
                schema: "Healthcare",
                table: "Insurances");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientServices_Patients_PatientId",
                schema: "Healthcare",
                table: "PatientServices");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "Healthcare",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "Healthcare",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "Healthcare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "Healthcare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "Healthcare",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "Healthcare",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "Healthcare",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Healthcare",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3960));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3967));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3970));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3973));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3975));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3979));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3981));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3983));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3985));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3987));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3990));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3992));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3994));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3996));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(3998));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(4001));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(4003));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(4005));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(4009));

            migrationBuilder.UpdateData(
                schema: "Healthcare",
                table: "Medicines",
                keyColumn: "MedicineId",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 26, 17, 28, 35, 12, DateTimeKind.Local).AddTicks(4011));

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "Healthcare",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Healthcare",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "Healthcare",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "Healthcare",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "Healthcare",
                table: "AspNetUserRoles",
                column: "RoleId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                schema: "Healthcare",
                table: "Admissions",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Patients_PatientId",
                schema: "Healthcare",
                table: "Bills",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Insurances_Patients_PatientId",
                schema: "Healthcare",
                table: "Insurances",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientServices_Patients_PatientId",
                schema: "Healthcare",
                table: "PatientServices",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                schema: "Healthcare",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Patients_PatientId",
                schema: "Healthcare",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK_Insurances_Patients_PatientId",
                schema: "Healthcare",
                table: "Insurances");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientServices_Patients_PatientId",
                schema: "Healthcare",
                table: "PatientServices");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "Healthcare");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "Healthcare");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                schema: "Healthcare",
                table: "Admissions",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Patients_PatientId",
                schema: "Healthcare",
                table: "Bills",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Insurances_Patients_PatientId",
                schema: "Healthcare",
                table: "Insurances",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientServices_Patients_PatientId",
                schema: "Healthcare",
                table: "PatientServices",
                column: "PatientId",
                principalSchema: "Healthcare",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
