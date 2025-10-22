using System.Text;
using System.Text.Json;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;

namespace TravelOperation.Core.Services;

public class ExportService : IExportService
{
    public async Task<byte[]> ExportTransactionsToCsvAsync(IEnumerable<Transaction> transactions)
    {
        return await Task.Run(() =>
        {
            var csv = new StringBuilder();
        csv.AppendLine("Transaction ID,Source,Email,Date,Authorization Date,Type,Category,Vendor,Address,Currency,Amount,Amount USD,Exchange Rate,Participants,Trip,Notes,Valid,Created,Modified By");

        foreach (var transaction in transactions)
        {
            csv.AppendLine($"\"{transaction.TransactionId}\"," +
                          $"\"{transaction.Source?.Name ?? ""}\"," +
                          $"\"{transaction.Email}\"," +
                          $"\"{transaction.TransactionDate:dd/MM/yyyy}\"," +
                          $"\"{transaction.AuthorizationDate?.ToString("dd/MM/yyyy") ?? ""}\"," +
                          $"\"{transaction.TransactionType ?? ""}\"," +
                          $"\"{transaction.Category?.Name ?? ""}\"," +
                          $"\"{transaction.Vendor ?? ""}\"," +
                          $"\"{transaction.Address ?? ""}\"," +
                          $"\"{transaction.Currency}\"," +
                          $"{transaction.Amount:N2}," +
                          $"{transaction.AmountUSD?.ToString("N2") ?? ""}," +
                          $"{transaction.ExchangeRate?.ToString("N6") ?? ""}," +
                          $"\"{transaction.Participants ?? ""}\"," +
                          $"\"{transaction.Trip?.TripName ?? ""}\"," +
                          $"\"{transaction.Notes ?? ""}\"," +
                          $"{transaction.IsValid}," +
                          $"\"{transaction.CreatedAt:dd/MM/yyyy HH:mm:ss}\"," +
                          $"\"{transaction.ModifiedBy ?? ""}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
        });
    }

    public async Task<byte[]> ExportTransactionsToExcelAsync(IEnumerable<Transaction> transactions)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transactions");

        // Headers
        var headers = new[] { "Transaction ID", "Source", "Email", "Date", "Authorization Date", "Type", 
                            "Category", "Vendor", "Address", "Currency", "Amount", "Amount USD", 
                            "Exchange Rate", "Participants", "Trip", "Notes", "Valid", "Created", "Modified By" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        int row = 2;
        foreach (var transaction in transactions)
        {
            worksheet.Cell(row, 1).Value = transaction.TransactionId;
            worksheet.Cell(row, 2).Value = transaction.Source?.Name ?? "";
            worksheet.Cell(row, 3).Value = transaction.Email;
            worksheet.Cell(row, 4).Value = transaction.TransactionDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 5).Value = transaction.AuthorizationDate?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cell(row, 6).Value = transaction.TransactionType ?? "";
            worksheet.Cell(row, 7).Value = transaction.Category?.Name ?? "";
            worksheet.Cell(row, 8).Value = transaction.Vendor ?? "";
            worksheet.Cell(row, 9).Value = transaction.Address ?? "";
            worksheet.Cell(row, 10).Value = transaction.Currency;
            worksheet.Cell(row, 11).Value = transaction.Amount;
            worksheet.Cell(row, 12).Value = transaction.AmountUSD ?? 0;
            worksheet.Cell(row, 13).Value = transaction.ExchangeRate ?? 0;
            worksheet.Cell(row, 14).Value = transaction.Participants ?? "";
            worksheet.Cell(row, 15).Value = transaction.Trip?.TripName ?? "";
            worksheet.Cell(row, 16).Value = transaction.Notes ?? "";
            worksheet.Cell(row, 17).Value = transaction.IsValid;
            worksheet.Cell(row, 18).Value = transaction.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cell(row, 19).Value = transaction.ModifiedBy ?? "";
            row++;
        }

        worksheet.ColumnsUsed().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportTripsToCsvAsync(IEnumerable<Trip> trips)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Trip ID,Trip Name,Email,Start Date,End Date,Duration,Country 1,City 1,Country 2,City 2,Purpose,Type,Status,Owner,Created,Modified By");

        foreach (var trip in trips)
        {
            csv.AppendLine($"{trip.TripId}," +
                          $"\"{trip.TripName}\"," +
                          $"\"{trip.Email}\"," +
                          $"\"{trip.StartDate:dd/MM/yyyy}\"," +
                          $"\"{trip.EndDate:dd/MM/yyyy}\"," +
                          $"{trip.Duration}," +
                          $"\"{trip.Country1}\"," +
                          $"\"{trip.City1}\"," +
                          $"\"{trip.Country2 ?? ""}\"," +
                          $"\"{trip.City2 ?? ""}\"," +
                          $"\"{trip.Purpose?.Name ?? ""}\"," +
                          $"\"{trip.TripType?.Name ?? ""}\"," +
                          $"\"{trip.Status?.Name ?? ""}\"," +
                          $"\"{trip.Owner?.Name ?? ""}\"," +
                          $"\"{trip.CreatedAt:dd/MM/yyyy HH:mm:ss}\"," +
                          $"\"{trip.ModifiedBy ?? ""}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> ExportTripsToExcelAsync(IEnumerable<Trip> trips)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Trips");

        // Headers
        var headers = new[] { "Trip ID", "Trip Name", "Email", "Start Date", "End Date", "Duration", 
                            "Country 1", "City 1", "Country 2", "City 2", "Purpose", "Type", 
                            "Status", "Owner", "Created", "Modified By" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        int row = 2;
        foreach (var trip in trips)
        {
            worksheet.Cell(row, 1).Value = trip.TripId;
            worksheet.Cell(row, 2).Value = trip.TripName;
            worksheet.Cell(row, 3).Value = trip.Email;
            worksheet.Cell(row, 4).Value = trip.StartDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 5).Value = trip.EndDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 6).Value = trip.Duration;
            worksheet.Cell(row, 7).Value = trip.Country1;
            worksheet.Cell(row, 8).Value = trip.City1;
            worksheet.Cell(row, 9).Value = trip.Country2 ?? "";
            worksheet.Cell(row, 10).Value = trip.City2 ?? "";
            worksheet.Cell(row, 11).Value = trip.Purpose?.Name ?? "";
            worksheet.Cell(row, 12).Value = trip.TripType?.Name ?? "";
            worksheet.Cell(row, 13).Value = trip.Status?.Name ?? "";
            worksheet.Cell(row, 14).Value = trip.Owner?.Name ?? "";
            worksheet.Cell(row, 15).Value = trip.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cell(row, 16).Value = trip.ModifiedBy ?? "";
            row++;
        }

        worksheet.ColumnsUsed().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportTravelSpendToCsvAsync(IEnumerable<TravelSpendReportItem> travelSpendData)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Trip Name,Email,Start Date,End Date,Duration,Countries,Purpose,Status,Transactions Count,Total Amount USD,Cost per Day,Airfare USD,Cabin Classes,Lodging USD,Lodging per Night,Meals USD,Meals per Day,Transportation USD,Client Entertainment USD,Communication USD,Other USD,Tax Exposure USD,Owner");

        foreach (var item in travelSpendData)
        {
            csv.AppendLine($"\"{item.TripName}\"," +
                          $"\"{item.Email}\"," +
                          $"\"{item.StartDate:dd/MM/yyyy}\"," +
                          $"\"{item.EndDate:dd/MM/yyyy}\"," +
                          $"{item.Duration}," +
                          $"\"{item.Countries}\"," +
                          $"\"{item.Purpose}\"," +
                          $"\"{item.Status}\"," +
                          $"{item.TransactionCount}," +
                          $"{item.TotalAmountUSD:N2}," +
                          $"{item.CostPerDay:N2}," +
                          $"{item.AirfareUSD:N2}," +
                          $"\"{item.CabinClasses}\"," +
                          $"{item.LodgingUSD:N2}," +
                          $"{item.LodgingPerNight:N2}," +
                          $"{item.MealsUSD:N2}," +
                          $"{item.MealsPerDay:N2}," +
                          $"{item.TransportationUSD:N2}," +
                          $"{item.ClientEntertainmentUSD:N2}," +
                          $"{item.CommunicationUSD:N2}," +
                          $"{item.OtherUSD:N2}," +
                          $"{item.TaxExposure:N2}," +
                          $"\"{item.Owner}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> ExportTravelSpendToExcelAsync(IEnumerable<TravelSpendReportItem> travelSpendData)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Travel Spend Report");

        // Headers
        var headers = new[] { "Trip Name", "Email", "Start Date", "End Date", "Duration", "Countries", 
                            "Purpose", "Status", "# Transactions", "Total Amount ($)", "Cost per Day ($)", 
                            "Airfare ($)", "Cabin Classes", "Lodging ($)", "Lodging per Night ($)", 
                            "Meals ($)", "Meals per Day ($)", "Transportation ($)", "Client Entertainment ($)", 
                            "Communication ($)", "Other ($)", "Tax Exposure ($)", "Owner" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        // Data
        int row = 2;
        foreach (var item in travelSpendData)
        {
            worksheet.Cell(row, 1).Value = item.TripName;
            worksheet.Cell(row, 2).Value = item.Email;
            worksheet.Cell(row, 3).Value = item.StartDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 4).Value = item.EndDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 5).Value = item.Duration;
            worksheet.Cell(row, 6).Value = item.Countries;
            worksheet.Cell(row, 7).Value = item.Purpose;
            worksheet.Cell(row, 8).Value = item.Status;
            worksheet.Cell(row, 9).Value = item.TransactionCount;
            worksheet.Cell(row, 10).Value = item.TotalAmountUSD;
            worksheet.Cell(row, 11).Value = item.CostPerDay;
            worksheet.Cell(row, 12).Value = item.AirfareUSD;
            worksheet.Cell(row, 13).Value = item.CabinClasses;
            worksheet.Cell(row, 14).Value = item.LodgingUSD;
            worksheet.Cell(row, 15).Value = item.LodgingPerNight;
            worksheet.Cell(row, 16).Value = item.MealsUSD;
            worksheet.Cell(row, 17).Value = item.MealsPerDay;
            worksheet.Cell(row, 18).Value = item.TransportationUSD;
            worksheet.Cell(row, 19).Value = item.ClientEntertainmentUSD;
            worksheet.Cell(row, 20).Value = item.CommunicationUSD;
            worksheet.Cell(row, 21).Value = item.OtherUSD;
            worksheet.Cell(row, 22).Value = item.TaxExposure;
            worksheet.Cell(row, 23).Value = item.Owner;
            row++;
        }

        // Format currency columns
        var currencyColumns = new[] { 10, 11, 12, 14, 15, 16, 17, 18, 19, 20, 21, 22 };
        foreach (var col in currencyColumns)
        {
            worksheet.Range(2, col, row - 1, col).Style.NumberFormat.Format = "$#,##0.00";
        }

        worksheet.ColumnsUsed().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportTravelSpendToPdfAsync(IEnumerable<TravelSpendReportItem> travelSpendData, TravelSpendSummary summary)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Title
        document.Add(new Paragraph("Travel Spend Report")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20)
            .SetBold());

        document.Add(new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(10));

        // Summary Section
        document.Add(new Paragraph("Summary")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        var summaryTable = new Table(new float[] { 1, 1 });
        summaryTable.SetWidth(UnitValue.CreatePercentValue(50));

        summaryTable.AddCell(new Cell().Add(new Paragraph("Total Trips:")).SetBold());
        summaryTable.AddCell(new Cell().Add(new Paragraph(summary.TotalTrips.ToString())));

        summaryTable.AddCell(new Cell().Add(new Paragraph("Total Amount:")).SetBold());
        summaryTable.AddCell(new Cell().Add(new Paragraph($"${summary.TotalAmount:N2}")));

        summaryTable.AddCell(new Cell().Add(new Paragraph("Average Cost per Trip:")).SetBold());
        summaryTable.AddCell(new Cell().Add(new Paragraph($"${summary.AverageCostPerTrip:N2}")));

        summaryTable.AddCell(new Cell().Add(new Paragraph("Total Tax Exposure:")).SetBold());
        summaryTable.AddCell(new Cell().Add(new Paragraph($"${summary.TotalTaxExposure:N2}")));

        document.Add(summaryTable);

        // Detailed Table
        document.Add(new Paragraph("Detailed Breakdown")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        var table = new Table(new float[] { 2, 1.5f, 1, 1, 1, 1.5f, 1, 1 });
        table.SetWidth(UnitValue.CreatePercentValue(100));

        // Headers
        var headers = new[] { "Trip", "Email", "Duration", "Total ($)", "Airfare ($)", "Lodging ($)", "Meals ($)", "Tax Exp. ($)" };
        foreach (var header in headers)
        {
            table.AddHeaderCell(new Cell().Add(new Paragraph(header))
                .SetBold()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER));
        }

        // Data rows
        foreach (var item in travelSpendData.Take(50)) // Limit for PDF readability
        {
            table.AddCell(new Cell().Add(new Paragraph(item.TripName)));
            table.AddCell(new Cell().Add(new Paragraph(item.Email)));
            table.AddCell(new Cell().Add(new Paragraph($"{item.Duration} days")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph($"${item.TotalAmountUSD:N0}")).SetTextAlignment(TextAlignment.RIGHT));
            table.AddCell(new Cell().Add(new Paragraph($"${item.AirfareUSD:N0}")).SetTextAlignment(TextAlignment.RIGHT));
            table.AddCell(new Cell().Add(new Paragraph($"${item.LodgingUSD:N0}")).SetTextAlignment(TextAlignment.RIGHT));
            table.AddCell(new Cell().Add(new Paragraph($"${item.MealsUSD:N0}")).SetTextAlignment(TextAlignment.RIGHT));
            table.AddCell(new Cell().Add(new Paragraph($"${item.TaxExposure:N0}")).SetTextAlignment(TextAlignment.RIGHT));
        }

        document.Add(table);

        if (travelSpendData.Count() > 50)
        {
            document.Add(new Paragraph($"Note: Showing first 50 of {travelSpendData.Count()} total trips")
                .SetFontSize(8)
                .SetItalic()
                .SetMarginTop(10));
        }

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportAuditLogToCsvAsync(IEnumerable<AuditLog> auditLogs)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Timestamp,User,Action,Table,Record ID,Old Value,New Value,Comments");

        foreach (var log in auditLogs)
        {
            csv.AppendLine($"\"{log.Timestamp:dd/MM/yyyy HH:mm:ss}\"," +
                          $"\"{log.UserId}\"," +
                          $"\"{log.Action}\"," +
                          $"\"{log.LinkedTable}\"," +
                          $"\"{log.LinkedRecordId}\"," +
                          $"\"{log.OldValue?.Replace("\"", "\"\"") ?? ""}\"," +
                          $"\"{log.NewValue?.Replace("\"", "\"\"") ?? ""}\"," +
                          $"\"{log.Comments?.Replace("\"", "\"\"") ?? ""}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> ExportAuditLogToExcelAsync(IEnumerable<AuditLog> auditLogs)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Audit Log");

        // Headers
        var headers = new[] { "Timestamp", "User", "Action", "Table", "Record ID", "Old Value", "New Value", "Comments" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.Orange;
        }

        // Data
        int row = 2;
        foreach (var log in auditLogs)
        {
            worksheet.Cell(row, 1).Value = log.Timestamp.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cell(row, 2).Value = log.UserId;
            worksheet.Cell(row, 3).Value = log.Action;
            worksheet.Cell(row, 4).Value = log.LinkedTable;
            worksheet.Cell(row, 5).Value = log.LinkedRecordId;
            worksheet.Cell(row, 6).Value = log.OldValue ?? "";
            worksheet.Cell(row, 7).Value = log.NewValue ?? "";
            worksheet.Cell(row, 8).Value = log.Comments ?? "";
            row++;
        }

        worksheet.ColumnsUsed().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportTaxComplianceReportToCsvAsync(IEnumerable<TaxComplianceReportItem> taxData)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Trip Name,Email,Start Date,End Date,Country,Meals Amount,Lodging Amount,Airfare Amount,Cabin Classes,Meals Cap,Lodging Cap,Meals Exposure,Lodging Exposure,Total Tax Exposure,Business Class Travel");

        foreach (var item in taxData)
        {
            csv.AppendLine($"\"{item.TripName}\"," +
                          $"\"{item.Email}\"," +
                          $"\"{item.StartDate:dd/MM/yyyy}\"," +
                          $"\"{item.EndDate:dd/MM/yyyy}\"," +
                          $"\"{item.Country}\"," +
                          $"{item.MealsAmount:N2}," +
                          $"{item.LodgingAmount:N2}," +
                          $"{item.AirfareAmount:N2}," +
                          $"\"{item.CabinClasses}\"," +
                          $"{item.MealsCap:N2}," +
                          $"{item.LodgingCap:N2}," +
                          $"{item.MealsExposure:N2}," +
                          $"{item.LodgingExposure:N2}," +
                          $"{item.TotalTaxExposure:N2}," +
                          $"{item.HasBusinessClassTravel}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> ExportTaxComplianceReportToExcelAsync(IEnumerable<TaxComplianceReportItem> taxData)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tax Compliance Report");

        // Headers
        var headers = new[] { "Trip Name", "Email", "Start Date", "End Date", "Country", "Meals Amount", 
                            "Lodging Amount", "Airfare Amount", "Cabin Classes", "Meals Cap", "Lodging Cap", 
                            "Meals Exposure", "Lodging Exposure", "Total Tax Exposure", "Business Class Travel" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.Red;
            worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }

        // Data
        int row = 2;
        foreach (var item in taxData)
        {
            worksheet.Cell(row, 1).Value = item.TripName;
            worksheet.Cell(row, 2).Value = item.Email;
            worksheet.Cell(row, 3).Value = item.StartDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 4).Value = item.EndDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 5).Value = item.Country;
            worksheet.Cell(row, 6).Value = item.MealsAmount;
            worksheet.Cell(row, 7).Value = item.LodgingAmount;
            worksheet.Cell(row, 8).Value = item.AirfareAmount;
            worksheet.Cell(row, 9).Value = item.CabinClasses;
            worksheet.Cell(row, 10).Value = item.MealsCap;
            worksheet.Cell(row, 11).Value = item.LodgingCap;
            worksheet.Cell(row, 12).Value = item.MealsExposure;
            worksheet.Cell(row, 13).Value = item.LodgingExposure;
            worksheet.Cell(row, 14).Value = item.TotalTaxExposure;
            worksheet.Cell(row, 15).Value = item.HasBusinessClassTravel ? "Yes" : "No";
            row++;
        }

        // Format currency columns
        var currencyColumns = new[] { 6, 7, 8, 10, 11, 12, 13, 14 };
        foreach (var col in currencyColumns)
        {
            worksheet.Range(2, col, row - 1, col).Style.NumberFormat.Format = "$#,##0.00";
        }

        // Highlight high tax exposure
        for (int r = 2; r < row; r++)
        {
            var cell = worksheet.Cell(r, 14);
            if (!cell.IsEmpty() && decimal.TryParse(cell.GetString(), out var exposure) && exposure > 1000)
            {
                cell.Style.Fill.BackgroundColor = XLColor.Yellow;
            }
        }

        worksheet.ColumnsUsed().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportTaxComplianceReportToPdfAsync(IEnumerable<TaxComplianceReportItem> taxData)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Title
        document.Add(new Paragraph("Tax Compliance Report")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20)
            .SetBold());

        document.Add(new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(10));

        // Summary
        var totalExposure = taxData.Sum(x => x.TotalTaxExposure);
        var highRiskTrips = taxData.Count(x => x.TotalTaxExposure > 1000);
        var businessClassTrips = taxData.Count(x => x.HasBusinessClassTravel);

        document.Add(new Paragraph("Executive Summary")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        document.Add(new Paragraph($"• Total Tax Exposure: ${totalExposure:N2}")
            .SetMarginLeft(20));
        document.Add(new Paragraph($"• High Risk Trips (>$1,000): {highRiskTrips}")
            .SetMarginLeft(20));
        document.Add(new Paragraph($"• Business Class Trips: {businessClassTrips}")
            .SetMarginLeft(20));

        // Detailed table for high-risk trips
        var highRiskData = taxData.Where(x => x.TotalTaxExposure > 100).OrderByDescending(x => x.TotalTaxExposure);

        if (highRiskData.Any())
        {
            document.Add(new Paragraph("High Risk Trips (Tax Exposure > $100)")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(20));

            var table = new Table(new float[] { 2, 1.5f, 1, 1, 1, 1 });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Headers
            var headers = new[] { "Trip", "Email", "Country", "Meals Exp.", "Lodging Exp.", "Total Exp." };
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header))
                    .SetBold()
                    .SetBackgroundColor(ColorConstants.RED)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetTextAlignment(TextAlignment.CENTER));
            }

            // Data rows
            foreach (var item in highRiskData.Take(30))
            {
                table.AddCell(new Cell().Add(new Paragraph(item.TripName)));
                table.AddCell(new Cell().Add(new Paragraph(item.Email)));
                table.AddCell(new Cell().Add(new Paragraph(item.Country)));
                table.AddCell(new Cell().Add(new Paragraph($"${item.MealsExposure:N0}")).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph($"${item.LodgingExposure:N0}")).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph($"${item.TotalTaxExposure:N0}")).SetTextAlignment(TextAlignment.RIGHT)
                    .SetBackgroundColor(item.TotalTaxExposure > 1000 ? ColorConstants.YELLOW : ColorConstants.WHITE));
            }

            document.Add(table);
        }

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportMonthlyReportToPdfAsync(MonthlyReportData reportData)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Title
        document.Add(new Paragraph(reportData.ReportTitle)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20)
            .SetBold());

        document.Add(new Paragraph($"Report Month: {reportData.ReportMonth:MMMM yyyy}")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(14));

        document.Add(new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(10));

        // Financial Summary
        document.Add(new Paragraph("Financial Summary")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        var finTable = new Table(new float[] { 1, 1 });
        finTable.SetWidth(UnitValue.CreatePercentValue(60));

        finTable.AddCell(new Cell().Add(new Paragraph("Money In:")).SetBold());
        finTable.AddCell(new Cell().Add(new Paragraph($"${reportData.TotalMoneyIn:N2}")));

        finTable.AddCell(new Cell().Add(new Paragraph("Money Out:")).SetBold());
        finTable.AddCell(new Cell().Add(new Paragraph($"${reportData.TotalMoneyOut:N2}")));

        finTable.AddCell(new Cell().Add(new Paragraph("Budget:")).SetBold());
        finTable.AddCell(new Cell().Add(new Paragraph($"${reportData.Budget:N2}")));

        finTable.AddCell(new Cell().Add(new Paragraph("Budget Remaining:")).SetBold());
        finTable.AddCell(new Cell().Add(new Paragraph($"${reportData.BudgetRemaining:N2}")
            .SetFontColor(reportData.BudgetRemaining < 0 ? ColorConstants.RED : ColorConstants.GREEN)));

        finTable.AddCell(new Cell().Add(new Paragraph("Budget Used:")).SetBold());
        finTable.AddCell(new Cell().Add(new Paragraph($"{reportData.BudgetSpentPercentage:N1}%")));

        document.Add(finTable);

        // Development Hours
        document.Add(new Paragraph("Development Hours")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        var hoursTable = new Table(new float[] { 1, 1 });
        hoursTable.SetWidth(UnitValue.CreatePercentValue(60));

        hoursTable.AddCell(new Cell().Add(new Paragraph("Planned Hours:")).SetBold());
        hoursTable.AddCell(new Cell().Add(new Paragraph(reportData.PlannedHours.ToString())));

        hoursTable.AddCell(new Cell().Add(new Paragraph("Actual Hours:")).SetBold());
        hoursTable.AddCell(new Cell().Add(new Paragraph(reportData.ActualHours.ToString())));

        hoursTable.AddCell(new Cell().Add(new Paragraph("Hours Used:")).SetBold());
        hoursTable.AddCell(new Cell().Add(new Paragraph($"{reportData.HoursUsedPercentage:N1}%")));

        document.Add(hoursTable);

        // Expense Breakdown
        if (reportData.ExpenseBreakdown.Any())
        {
            document.Add(new Paragraph("Expense Breakdown")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(20));

            var expTable = new Table(new float[] { 2, 1, 1 });
            expTable.SetWidth(UnitValue.CreatePercentValue(80));

            expTable.AddHeaderCell(new Cell().Add(new Paragraph("Category")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            expTable.AddHeaderCell(new Cell().Add(new Paragraph("Amount")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            expTable.AddHeaderCell(new Cell().Add(new Paragraph("Percentage")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));

            foreach (var expense in reportData.ExpenseBreakdown)
            {
                expTable.AddCell(new Cell().Add(new Paragraph(expense.Category)));
                expTable.AddCell(new Cell().Add(new Paragraph($"${expense.Amount:N2}")).SetTextAlignment(TextAlignment.RIGHT));
                expTable.AddCell(new Cell().Add(new Paragraph($"{expense.Percentage:N1}%")).SetTextAlignment(TextAlignment.RIGHT));
            }

            document.Add(expTable);
        }

        // Project Progress
        document.Add(new Paragraph("Project Progress")
            .SetFontSize(16)
            .SetBold()
            .SetMarginTop(20));

        var progressTable = new Table(new float[] { 1, 1 });
        progressTable.SetWidth(UnitValue.CreatePercentValue(60));

        progressTable.AddCell(new Cell().Add(new Paragraph("Milestones Completed:")).SetBold());
        progressTable.AddCell(new Cell().Add(new Paragraph($"{reportData.MilestonesCompletedPercentage:N1}%")));

        progressTable.AddCell(new Cell().Add(new Paragraph("Overall Progress:")).SetBold());
        progressTable.AddCell(new Cell().Add(new Paragraph($"{reportData.OverallProgressPercentage:N1}%"))
            .SetBold()
            .SetFontSize(14)
            .SetFontColor(reportData.OverallProgressPercentage >= 80 ? ColorConstants.GREEN : 
                         reportData.OverallProgressPercentage >= 60 ? ColorConstants.ORANGE : ColorConstants.RED));

        document.Add(progressTable);

        // Notes
        if (!string.IsNullOrEmpty(reportData.Notes))
        {
            document.Add(new Paragraph("Notes")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(20));

            document.Add(new Paragraph(reportData.Notes)
                .SetMarginLeft(20));
        }

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<string> GetExportFileName(string reportType, string format, DateTime? reportDate = null)
    {
        var timestamp = reportDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
        var cleanReportType = reportType.Replace(" ", "_").ToLower();
        
        return await Task.FromResult($"{cleanReportType}_{timestamp}.{format.ToLower()}");
    }
}