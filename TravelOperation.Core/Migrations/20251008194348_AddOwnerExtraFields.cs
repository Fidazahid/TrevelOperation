using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelOperation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSplit",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginalTransactionId",
                table: "Transactions",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Owners",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManagerEmail",
                table: "Owners",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Owners",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Owners",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CountriesCities",
                columns: table => new
                {
                    CountryCityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountriesCities", x => x.CountryCityId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ManagerEmail = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CostCenter = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MonthlyCreditLimit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Period = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ApplicableRegion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdditionalRules = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.PolicyId);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SystemSettingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SystemSettingId);
                });

            migrationBuilder.CreateTable(
                name: "TripRequests",
                columns: table => new
                {
                    TripRequestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BusinessJustification = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRequests", x => x.TripRequestId);
                    table.ForeignKey(
                        name: "FK_TripRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TripRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedByEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_Approvals_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Approvals_TripRequests_TripRequestId",
                        column: x => x.TripRequestId,
                        principalTable: "TripRequests",
                        principalColumn: "TripRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TripRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AmountUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VendorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ReceiptPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsReimbursable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReimbursedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_TripRequests_TripRequestId",
                        column: x => x.TripRequestId,
                        principalTable: "TripRequests",
                        principalColumn: "TripRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CountriesCities",
                columns: new[] { "CountryCityId", "City", "Country", "CreatedAt", "ModifiedAt", "ModifiedBy" },
                values: new object[,]
                {
                    { 1, "Tel Aviv", "Israel", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 2, "Jerusalem", "Israel", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 3, "New York", "United States", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 4, "San Francisco", "United States", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 5, "London", "United Kingdom", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 6, "Berlin", "Germany", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" },
                    { 7, "Paris", "France", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "CostCenter", "CreatedAt", "Department", "Email", "IsActive", "ManagerEmail", "ModifiedAt", "MonthlyCreditLimit", "Name", "Role", "Title" },
                values: new object[,]
                {
                    { 1, "CC-ENG-001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "john.doe@company.com", true, "sarah.manager@company.com", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5000.00m, "John Doe", "Employee", "Software Engineer" },
                    { 2, "CC-ENG-001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "sara.manager@company.com", true, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5000.00m, "Sara Manager", "Manager", "Engineering Manager" },
                    { 3, "CC-FIN-001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "finance@company.com", true, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5000.00m, "Finance Officer", "Finance", "Finance Officer" },
                    { 4, "CC-OPS-001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "admin@company.com", true, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5000.00m, "Admin User", "Admin", "System Administrator" }
                });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "OwnerId",
                keyValue: 1,
                columns: new[] { "IsActive", "ManagerEmail", "Notes", "Title" },
                values: new object[] { true, null, null, null });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "OwnerId",
                keyValue: 2,
                columns: new[] { "IsActive", "ManagerEmail", "Notes", "Title" },
                values: new object[] { true, null, null, null });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "PolicyId", "AdditionalRules", "ApplicableRegion", "Category", "CreatedAt", "Description", "EffectiveFrom", "EffectiveTo", "IsActive", "MaxAmount", "ModifiedAt", "Period" },
                values: new object[,]
                {
                    { 1, null, "Global", "Travel", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flight bookings", new DateTime(2025, 10, 8, 19, 43, 45, 419, DateTimeKind.Utc).AddTicks(7117), null, true, 1500.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Per Trip" },
                    { 2, null, "Global", "Meals", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily meal allowance", new DateTime(2025, 10, 8, 19, 43, 45, 420, DateTimeKind.Utc).AddTicks(4367), null, true, 75.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" },
                    { 3, null, "Global", "Accommodation", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel accommodation", new DateTime(2025, 10, 8, 19, 43, 45, 420, DateTimeKind.Utc).AddTicks(4382), null, true, 200.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" },
                    { 4, null, "Global", "Miscellaneous", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Other travel expenses", new DateTime(2025, 10, 8, 19, 43, 45, 420, DateTimeKind.Utc).AddTicks(4390), null, true, 100.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "SystemSettingId", "CreatedAt", "Description", "Key", "ModifiedAt", "ModifiedBy", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Default currency for calculations", "DefaultCurrency", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "USD" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Date display format", "DateFormat", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "dd/MM/yyyy" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Threshold for flagging high-value meals", "HighValueMealThreshold", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "80" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Threshold for flagging low-value lodging", "LowValueLodgingThreshold", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "100" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ApprovedByEmployeeId",
                table: "Approvals",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_Status",
                table: "Approvals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_TripRequestId",
                table: "Approvals",
                column: "TripRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CountriesCities_Country_City",
                table: "CountriesCities",
                columns: new[] { "Country", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Role",
                table: "Employees",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Category",
                table: "Expenses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EmployeeId",
                table: "Expenses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TripRequestId",
                table: "Expenses",
                column: "TripRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_Category",
                table: "Policies",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_IsActive",
                table: "Policies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_EmployeeId",
                table: "TripRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_StartDate",
                table: "TripRequests",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_Status",
                table: "TripRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "CountriesCities");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TripRequests");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropColumn(
                name: "IsSplit",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OriginalTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "ManagerEmail",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Owners");
        }
    }
}
