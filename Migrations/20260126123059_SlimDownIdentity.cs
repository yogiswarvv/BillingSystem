using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SlimDownIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "AspNetUserTokens",
                schema: "Healthcare");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "Healthcare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                name: "IX_AspNetUserClaims_UserId",
                schema: "Healthcare",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "Healthcare",
                table: "AspNetUserLogins",
                column: "UserId");
        }
    }
}
