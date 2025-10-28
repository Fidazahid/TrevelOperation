using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelOperation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipientEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ActionUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ActionLabel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.InsertData(
                table: "AuthUsers",
                columns: new[] { "Email", "CreatedDate", "Department", "FirstName", "IsActive", "LastLoginDate", "LastName", "Password", "Role" },
                values: new object[] { "admin@company.com", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "System", true, null, "Administrator", "Admin@123", "Finance" });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 1,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 27, 16, 15, 27, 707, DateTimeKind.Utc).AddTicks(4210));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 2,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 27, 16, 15, 27, 707, DateTimeKind.Utc).AddTicks(7590));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 3,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 27, 16, 15, 27, 707, DateTimeKind.Utc).AddTicks(7599));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 4,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 27, 16, 15, 27, 707, DateTimeKind.Utc).AddTicks(7602));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthUsers");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 1,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 9, 9, 37, 51, 691, DateTimeKind.Utc).AddTicks(6071));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 2,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 9, 9, 37, 51, 691, DateTimeKind.Utc).AddTicks(8631));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 3,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 9, 9, 37, 51, 691, DateTimeKind.Utc).AddTicks(8637));

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "PolicyId",
                keyValue: 4,
                column: "EffectiveFrom",
                value: new DateTime(2025, 10, 9, 9, 37, 51, 691, DateTimeKind.Utc).AddTicks(8640));
        }
    }
}
