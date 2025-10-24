using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelOperation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Transactions table indexes - most frequently queried columns
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Email",
                table: "Transactions",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TripId",
                table: "Transactions",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IsValid",
                table: "Transactions",
                column: "IsValid");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DataValidation",
                table: "Transactions",
                column: "DataValidation");

            // Composite index for common query pattern: unlinked transactions by date
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TripId_TransactionDate",
                table: "Transactions",
                columns: new[] { "TripId", "TransactionDate" });

            // Composite index for policy compliance queries
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId_AmountUSD_IsValid",
                table: "Transactions",
                columns: new[] { "CategoryId", "AmountUSD", "IsValid" });

            // Trips table indexes
            migrationBuilder.CreateIndex(
                name: "IX_Trips_Email",
                table: "Trips",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartDate",
                table: "Trips",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_EndDate",
                table: "Trips",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StatusId",
                table: "Trips",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ValidationStatusId",
                table: "Trips",
                column: "ValidationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_OwnerId",
                table: "Trips",
                column: "OwnerId");

            // Composite index for trip date range queries
            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartDate_EndDate",
                table: "Trips",
                columns: new[] { "StartDate", "EndDate" });

            // Composite index for trip status queries
            migrationBuilder.CreateIndex(
                name: "IX_Trips_StatusId_ValidationStatusId",
                table: "Trips",
                columns: new[] { "StatusId", "ValidationStatusId" });

            // Audit Log indexes for tracking and searching
            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Timestamp",
                table: "AuditLog",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_LinkedTable",
                table: "AuditLog",
                column: "LinkedTable");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_LinkedRecordId",
                table: "AuditLog",
                column: "LinkedRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserId",
                table: "AuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Action",
                table: "AuditLog",
                column: "Action");

            // Composite index for audit log queries
            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_LinkedTable_LinkedRecordId",
                table: "AuditLog",
                columns: new[] { "LinkedTable", "LinkedRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Timestamp_Action",
                table: "AuditLog",
                columns: new[] { "Timestamp", "Action" });

            // Headcount table indexes for employee lookups
            migrationBuilder.CreateIndex(
                name: "IX_Headcount_Email",
                table: "Headcount",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Headcount_Department",
                table: "Headcount",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Headcount_CostCenter",
                table: "Headcount",
                column: "CostCenter");

            // Tax table indexes for tax calculations
            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_FiscalYear_Country",
                table: "TaxRules",
                columns: new[] { "FiscalYear", "Country" });

            // Owners table index
            migrationBuilder.CreateIndex(
                name: "IX_Owners_Email",
                table: "Owners",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all indexes in reverse order
            migrationBuilder.DropIndex(name: "IX_Owners_Email", table: "Owners");
            
            migrationBuilder.DropIndex(name: "IX_TaxRules_FiscalYear_Country", table: "TaxRules");
            
            migrationBuilder.DropIndex(name: "IX_Headcount_Email", table: "Headcount");
            migrationBuilder.DropIndex(name: "IX_Headcount_Department", table: "Headcount");
            migrationBuilder.DropIndex(name: "IX_Headcount_CostCenter", table: "Headcount");
            
            migrationBuilder.DropIndex(name: "IX_AuditLog_Timestamp", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_LinkedTable", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_LinkedRecordId", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_UserId", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_Action", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_LinkedTable_LinkedRecordId", table: "AuditLog");
            migrationBuilder.DropIndex(name: "IX_AuditLog_Timestamp_Action", table: "AuditLog");
            
            migrationBuilder.DropIndex(name: "IX_Trips_Email", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_StartDate", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_EndDate", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_StatusId", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_ValidationStatusId", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_OwnerId", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_StartDate_EndDate", table: "Trips");
            migrationBuilder.DropIndex(name: "IX_Trips_StatusId_ValidationStatusId", table: "Trips");
            
            migrationBuilder.DropIndex(name: "IX_Transactions_Email", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_TransactionDate", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_CategoryId", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_TripId", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_IsValid", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_DataValidation", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_TripId_TransactionDate", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Transactions_CategoryId_AmountUSD_IsValid", table: "Transactions");
        }
    }
}
