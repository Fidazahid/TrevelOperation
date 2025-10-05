using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelOperation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdTracking : Migration
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
                name: "AuthUsers",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
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
                    Icon = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreatedByEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedByName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CostCenter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Domain = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ManagerEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerId);
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
                name: "TransformationRules",
                columns: table => new
                {
                    TransformationRuleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PolicyPattern = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRegex = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransformationRules", x => x.TransformationRuleId);
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
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
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
                    IsSplit = table.Column<bool>(type: "INTEGER", nullable: false),
                    OriginalTransactionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
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
                table: "AuthUsers",
                columns: new[] { "Email", "CreatedDate", "Department", "FirstName", "IsActive", "LastLoginDate", "LastName", "Password", "Role", "UserId" },
                values: new object[] { "admin@company.com", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "System", true, null, "Administrator", "Admin@123", "Finance", "44180cf6-e9bd-4096-8cbb-e01a88ef0c7d" });

            migrationBuilder.InsertData(
                table: "BookingStatuses",
                columns: new[] { "BookingStatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🔴", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Canceled" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🟢", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Approved" }
                });

            migrationBuilder.InsertData(
                table: "BookingTypes",
                columns: new[] { "BookingTypeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "✈️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flight" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🏨", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🚗", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Car" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🚆", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Train" }
                });

            migrationBuilder.InsertData(
                table: "CabinClasses",
                columns: new[] { "CabinClassId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "💺", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Economy" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🛫", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium economy" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🧳", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Business" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "👑", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "First" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "✈️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Airfare" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🏨", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lodging" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🚕", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Transportation" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "📱", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Communication" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🍸", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Client entertainment" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🍽️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Meals" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "❔", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Other" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "❓", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-travel" }
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

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "OwnerId", "CostCenter", "CreatedAt", "Department", "Domain", "Email", "IsActive", "ManagerEmail", "ModifiedAt", "Name", "Notes", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "maayan@company.com", true, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Maayan Chesler", null, null },
                    { 2, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "martina@company.com", true, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Martina Poplinsk", null, null }
                });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "PolicyId", "AdditionalRules", "ApplicableRegion", "Category", "CreatedAt", "Description", "EffectiveFrom", "EffectiveTo", "IsActive", "MaxAmount", "ModifiedAt", "Period" },
                values: new object[,]
                {
                    { 1, null, "Global", "Travel", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flight bookings", new DateTime(2025, 10, 5, 13, 13, 34, 259, DateTimeKind.Utc).AddTicks(267), null, true, 1500.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Per Trip" },
                    { 2, null, "Global", "Meals", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily meal allowance", new DateTime(2025, 10, 5, 13, 13, 34, 259, DateTimeKind.Utc).AddTicks(4372), null, true, 75.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" },
                    { 3, null, "Global", "Accommodation", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel accommodation", new DateTime(2025, 10, 5, 13, 13, 34, 259, DateTimeKind.Utc).AddTicks(4382), null, true, 200.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" },
                    { 4, null, "Global", "Miscellaneous", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Other travel expenses", new DateTime(2025, 10, 5, 13, 13, 34, 259, DateTimeKind.Utc).AddTicks(4388), null, true, 100.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily" }
                });

            migrationBuilder.InsertData(
                table: "Purposes",
                columns: new[] { "PurposeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "💼", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Business trip" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🎓", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Onboarding" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🏖️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Company trip" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🛡️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BCP" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "SourceId", "CreatedAt", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🧳", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Navan" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "👤", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agent" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "✏️", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manual" }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "StatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🔴", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Canceled" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "⚪", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Upcoming" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🔵", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ongoing" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🟢", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Completed" }
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

            migrationBuilder.InsertData(
                table: "TripTypes",
                columns: new[] { "TripTypeId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🏠", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Domestic" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🌍", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "International" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "📍", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Local" }
                });

            migrationBuilder.InsertData(
                table: "ValidationStatuses",
                columns: new[] { "ValidationStatusId", "CreatedAt", "Description", "Emoji", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "⚪", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Not ready to validate" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🟡", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ready to validate" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "🟢", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Validated" }
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
                name: "IX_TransformationRules_IsActive",
                table: "TransformationRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TransformationRules_Priority",
                table: "TransformationRules",
                column: "Priority",
                descending: new bool[0]);

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
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuthUsers");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "CountriesCities");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Headcount");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TaxRules");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TransformationRules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "TripRequests");

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
                name: "Employees");

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
