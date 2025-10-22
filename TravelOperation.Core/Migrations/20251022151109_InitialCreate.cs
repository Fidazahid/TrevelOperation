using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelOperation.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LinkedTable = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LinkedRecordId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "TEXT", nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", nullable: true),
                    Comments = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditId);
                });

            migrationBuilder.CreateTable(
                name: "BookingStatuses",
                columns: table => new
                {
                    BookingStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatuses", x => x.BookingStatusId);
                });

            migrationBuilder.CreateTable(
                name: "BookingTypes",
                columns: table => new
                {
                    BookingTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTypes", x => x.BookingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CabinClasses",
                columns: table => new
                {
                    CabinClassId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinClasses", x => x.CabinClassId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Headcount",
                columns: table => new
                {
                    HeadcountId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Period = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Subsidiary = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Site = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Domain = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CostCenter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headcount", x => x.HeadcountId);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CostCenter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Domain = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "Purposes",
                columns: table => new
                {
                    PurposeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purposes", x => x.PurposeId);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceId);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "TaxRules",
                columns: table => new
                {
                    TaxId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FiscalYear = table.Column<int>(type: "INTEGER", nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Subsidiary = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MealsCap = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LodgingCap = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxShield = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRules", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "TripTypes",
                columns: table => new
                {
                    TripTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripTypes", x => x.TripTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ValidationStatuses",
                columns: table => new
                {
                    ValidationStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationStatuses", x => x.ValidationStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TripName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    Country1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    City1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PurposeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TripTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidationStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsManual = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                    table.ForeignKey(
                        name: "FK_Trips_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_Purposes_PurposeId",
                        column: x => x.PurposeId,
                        principalTable: "Purposes",
                        principalColumn: "PurposeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_TripTypes_TripTypeId",
                        column: x => x.TripTypeId,
                        principalTable: "TripTypes",
                        principalColumn: "TripTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_ValidationStatuses_ValidationStatusId",
                        column: x => x.ValidationStatusId,
                        principalTable: "ValidationStatuses",
                        principalColumn: "ValidationStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SourceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorizationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransactionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Vendor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MerchantCategory = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SourceTripId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BookingId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BookingStatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    BookingStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BookingEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BookingTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Policy = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Participants = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    TripId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataValidation = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParticipantsValidated = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    CabinClassId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_BookingStatuses_BookingStatusId",
                        column: x => x.BookingStatusId,
                        principalTable: "BookingStatuses",
                        principalColumn: "BookingStatusId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_BookingTypes_BookingTypeId",
                        column: x => x.BookingTypeId,
                        principalTable: "BookingTypes",
                        principalColumn: "BookingTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_CabinClasses_CabinClassId",
                        column: x => x.CabinClassId,
                        principalTable: "CabinClasses",
                        principalColumn: "CabinClassId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "SourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "BookingStatuses",
                columns: new[] { "BookingStatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(5790), null, "🔴", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(6039), "Canceled" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(6287), null, "🟢", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(6287), "Approved" }
                });

            migrationBuilder.InsertData(
                table: "BookingTypes",
                columns: new[] { "BookingTypeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(3828), null, "✈️", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4084), "Flight" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4331), null, "🏨", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4331), "Hotel" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4333), null, "🚗", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4333), "Car" },
                    { 4, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4335), null, "🚆", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(4335), "Train" }
                });

            migrationBuilder.InsertData(
                table: "CabinClasses",
                columns: new[] { "CabinClassId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(3822), null, "💺", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4120), "Economy" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4402), null, "🛫", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4402), "Premium economy" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4404), null, "🧳", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4405), "Business" },
                    { 4, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4406), null, "👑", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(4407), "First" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8070), null, "✈️", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8407), "Airfare" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8711), null, "🏨", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8711), "Lodging" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8713), null, "🚕", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8714), "Transportation" },
                    { 4, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8715), null, "📱", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8716), "Communication" },
                    { 5, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8717), null, "🍸", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8718), "Client entertainment" },
                    { 6, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8720), null, "🍽️", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8720), "Meals" },
                    { 7, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8722), null, "❔", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8722), "Other" },
                    { 8, new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8724), null, "❓", new DateTime(2025, 10, 22, 15, 11, 8, 949, DateTimeKind.Utc).AddTicks(8724), "Non-travel" }
                });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "OwnerId", "CostCenter", "CreatedAt", "Department", "Domain", "Email", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(8227), null, null, "maayan@company.com", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(8489), "Maayan Chesler" },
                    { 2, null, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(8740), null, null, "martina@company.com", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(8741), "Martina Poplinsk" }
                });

            migrationBuilder.InsertData(
                table: "Purposes",
                columns: new[] { "PurposeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(1365), null, "💼", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(1667), "Business trip" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2014), null, "🎓", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2015), "Onboarding" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2017), null, "🏖️", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2018), "Company trip" },
                    { 4, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2019), null, "🛡️", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(2020), "BCP" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "SourceId", "CreatedAt", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(8355), "🧳", new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(8696), "Navan" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(9000), "👤", new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(9001), "Agent" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(9003), "✏️", new DateTime(2025, 10, 22, 15, 11, 8, 948, DateTimeKind.Utc).AddTicks(9004), "Manual" }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "StatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(8693), null, "🔴", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(8982), "Canceled" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9415), null, "⚪", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9416), "Upcoming" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9418), null, "🔵", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9418), "Ongoing" },
                    { 4, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9420), null, "🟢", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(9420), "Completed" }
                });

            migrationBuilder.InsertData(
                table: "TripTypes",
                columns: new[] { "TripTypeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6387), null, "🏠", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6686), "Domestic" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6972), null, "🌍", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6973), "International" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6975), null, "📍", new DateTime(2025, 10, 22, 15, 11, 8, 950, DateTimeKind.Utc).AddTicks(6976), "Local" }
                });

            migrationBuilder.InsertData(
                table: "ValidationStatuses",
                columns: new[] { "ValidationStatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1307), null, "⚪", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1606), "Not ready to validate" },
                    { 2, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1892), null, "🟡", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1893), "Ready to validate" },
                    { 3, new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1894), null, "🟢", new DateTime(2025, 10, 22, 15, 11, 8, 951, DateTimeKind.Utc).AddTicks(1895), "Validated" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_LinkedRecordId",
                table: "AuditLogs",
                column: "LinkedRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_LinkedTable",
                table: "AuditLogs",
                column: "LinkedTable");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingStatusId",
                table: "Transactions",
                column: "BookingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingTypeId",
                table: "Transactions",
                column: "BookingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CabinClassId",
                table: "Transactions",
                column: "CabinClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Email",
                table: "Transactions",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourceId",
                table: "Transactions",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TripId",
                table: "Transactions",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Email",
                table: "Trips",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_EndDate",
                table: "Trips",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_OwnerId",
                table: "Trips",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_PurposeId",
                table: "Trips",
                column: "PurposeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartDate",
                table: "Trips",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StatusId",
                table: "Trips",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripTypeId",
                table: "Trips",
                column: "TripTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ValidationStatusId",
                table: "Trips",
                column: "ValidationStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Headcount");

            migrationBuilder.DropTable(
                name: "TaxRules");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "BookingStatuses");

            migrationBuilder.DropTable(
                name: "BookingTypes");

            migrationBuilder.DropTable(
                name: "CabinClasses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Purposes");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "TripTypes");

            migrationBuilder.DropTable(
                name: "ValidationStatuses");
        }
    }
}
