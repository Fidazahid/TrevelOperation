using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Models.Entities;
using TransformationRuleEntity = TravelOperation.Core.Models.Entities.TransformationRule;
using TransformationRuleDto = TrevelOperation.Service.TransformationRule;

namespace TrevelOperation.Service;

public class CsvImportService : ICsvImportService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<CsvImportService> _logger;
    private static readonly List<TransformationRuleDto> DefaultRules = GetDefaultTransformationRules();

    public CsvImportService(TravelDbContext context, ILogger<CsvImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportNavanCsvAsync(Stream csvStream, string fileName)
    {
        var mapping = GetNavanFieldMapping();
        return await ProcessCsvImportAsync(csvStream, fileName, mapping, "Navan");
    }

    public async Task<ImportResult> ImportAgentCsvAsync(Stream csvStream, string fileName)
    {
        var mapping = GetAgentFieldMapping();
        return await ProcessCsvImportAsync(csvStream, fileName, mapping, "Agent");
    }

    public async Task<ImportResult> ImportManualCsvAsync(Stream csvStream, string fileName)
    {
        var mapping = GetManualFieldMapping();
        return await ProcessCsvImportAsync(csvStream, fileName, mapping, "Manual");
    }

    public async Task<List<TransformationRuleDto>> GetTransformationRulesAsync()
    {
        try
        {
            // Get rules from database
            var dbRules = await _context.TransformationRules
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.Priority)
                .ToListAsync();

            // If no rules in database, return default rules
            if (!dbRules.Any())
            {
                _logger.LogInformation("No transformation rules found in database, returning default rules");
                // Convert default rules to match the DTO structure expected by the UI
                return DefaultRules.Select(r => new TransformationRuleDto
                {
                    RuleId = r.RuleId,
                    PolicyPattern = r.PolicyPattern,
                    CategoryName = r.CategoryName,
                    Priority = r.Priority,
                    IsRegex = r.IsRegex,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    ModifiedAt = r.ModifiedAt,
                    CreatedBy = r.CreatedBy
                }).ToList();
            }

            // Convert entity to DTO
            return dbRules.Select(e => new TransformationRuleDto
            {
                RuleId = e.TransformationRuleId,
                PolicyPattern = e.PolicyPattern,
                CategoryName = e.CategoryName,
                Priority = e.Priority,
                IsRegex = e.IsRegex,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                ModifiedAt = e.ModifiedAt,
                CreatedBy = e.ModifiedBy
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transformation rules from database");
            return DefaultRules;
        }
    }

    public async Task SaveTransformationRulesAsync(List<TransformationRuleDto> rules)
    {
        try
        {
            // Clear existing rules
            var existingRules = await _context.TransformationRules.ToListAsync();
            _context.TransformationRules.RemoveRange(existingRules);

            // Add new rules
            var now = DateTime.UtcNow;
            var entities = rules.Select(r => new TransformationRuleEntity
            {
                PolicyPattern = r.PolicyPattern,
                CategoryName = r.CategoryName,
                Priority = r.Priority,
                IsRegex = r.IsRegex,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt == default ? now : r.CreatedAt,
                ModifiedAt = now,
                ModifiedBy = "System" // TODO: Get from current user context
            }).ToList();

            await _context.TransformationRules.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully saved {Count} transformation rules", rules.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving transformation rules to database");
            throw;
        }
        await Task.CompletedTask;
    }

    private async Task<ImportResult> ProcessCsvImportAsync(Stream csvStream, string fileName, CsvFieldMapping mapping, string source)
    {
        var result = new ImportResult
        {
            FileName = fileName,
            ImportDate = DateTime.UtcNow
        };

        try
        {
            var importedData = ParseCsvData(csvStream, mapping);
            result.RecordsProcessed = importedData.Count;

            var transformationRules = await GetTransformationRulesAsync();
            var sourceEntity = await GetOrCreateSourceAsync(source);

            foreach (var data in importedData)
            {
                try
                {
                    // Apply transformation rules to determine category
                    data.DeterminedCategory = ApplyTransformationRules(data.Policy, transformationRules);

                    // Validate the data
                    ValidateImportedData(data);

                    if (data.ValidationErrors.Any())
                    {
                        result.RecordsWithErrors++;
                        result.Errors.AddRange(data.ValidationErrors.Select(e => $"{data.TransactionId}: {e}"));
                        continue;
                    }

                    // Check if transaction already exists
                    var existingTransaction = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.TransactionId == data.TransactionId);

                    if (existingTransaction != null)
                    {
                        result.RecordsSkipped++;
                        result.Warnings.Add($"Transaction {data.TransactionId} already exists - skipped");
                        continue;
                    }

                    // Create new transaction
                    var transaction = await CreateTransactionFromImportData(data, sourceEntity);
                    _context.Transactions.Add(transaction);
                    result.RecordsImported++;
                }
                catch (Exception ex)
                {
                    result.RecordsWithErrors++;
                    result.Errors.Add($"Error processing {data.TransactionId}: {ex.Message}");
                    _logger.LogError(ex, "Error processing transaction {TransactionId}", data.TransactionId);
                }
            }

            await _context.SaveChangesAsync();
            result.Success = result.RecordsImported > 0;

            _logger.LogInformation("CSV import completed. File: {FileName}, Processed: {Processed}, Imported: {Imported}, Errors: {Errors}",
                fileName, result.RecordsProcessed, result.RecordsImported, result.RecordsWithErrors);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Import failed: {ex.Message}");
            _logger.LogError(ex, "CSV import failed for file {FileName}", fileName);
        }

        return result;
    }

    private List<ImportedTransactionData> ParseCsvData(Stream csvStream, CsvFieldMapping mapping)
    {
        var results = new List<ImportedTransactionData>();
        
        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        var lines = new List<string>();
        
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
        }

        if (lines.Count == 0)
        {
            return results;
        }

        var headerLine = lines[0];
        var headers = ParseCsvLine(headerLine, mapping.Delimiter);
        var dataStartIndex = mapping.HasHeader ? 1 : 0;

        var culture = new CultureInfo(mapping.CultureInfo);

        for (int i = dataStartIndex; i < lines.Count; i++)
        {
            try
            {
                var values = ParseCsvLine(lines[i], mapping.Delimiter);
                
                if (values.Count != headers.Count)
                {
                    _logger.LogWarning("Line {LineNumber} has {ValueCount} values but {HeaderCount} headers expected", 
                        i + 1, values.Count, headers.Count);
                    continue;
                }

                var data = new ImportedTransactionData();
                
                // Map values to properties based on field mapping
                for (int j = 0; j < headers.Count; j++)
                {
                    var headerName = headers[j];
                    var value = values[j];
                    
                    MapFieldValue(data, headerName, value, mapping, culture);
                }

                results.Add(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing line {LineNumber}: {Line}", i + 1, lines[i]);
            }
        }

        return results;
    }

    private void MapFieldValue(ImportedTransactionData data, string headerName, string value, CsvFieldMapping mapping, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        try
        {
            if (headerName.Equals(mapping.TransactionIdColumn, StringComparison.OrdinalIgnoreCase))
                data.TransactionId = value;
            else if (headerName.Equals(mapping.EmailColumn, StringComparison.OrdinalIgnoreCase))
                data.Email = value;
            else if (headerName.Equals(mapping.TransactionDateColumn, StringComparison.OrdinalIgnoreCase))
                data.TransactionDate = DateTime.ParseExact(value, mapping.DateFormat, culture);
            else if (headerName.Equals(mapping.AuthorizationDateColumn, StringComparison.OrdinalIgnoreCase))
                data.AuthorizationDate = DateTime.ParseExact(value, mapping.DateFormat, culture);
            else if (headerName.Equals(mapping.TransactionTypeColumn, StringComparison.OrdinalIgnoreCase))
                data.TransactionType = value;
            else if (headerName.Equals(mapping.VendorColumn, StringComparison.OrdinalIgnoreCase))
                data.Vendor = value;
            else if (headerName.Equals(mapping.MerchantCategoryColumn, StringComparison.OrdinalIgnoreCase))
                data.MerchantCategory = value;
            else if (headerName.Equals(mapping.AddressColumn, StringComparison.OrdinalIgnoreCase))
                data.Address = value;
            else if (headerName.Equals(mapping.SourceTripIdColumn, StringComparison.OrdinalIgnoreCase))
                data.SourceTripId = value;
            else if (headerName.Equals(mapping.BookingIdColumn, StringComparison.OrdinalIgnoreCase))
                data.BookingId = value;
            else if (headerName.Equals(mapping.BookingStartDateColumn, StringComparison.OrdinalIgnoreCase))
                data.BookingStartDate = DateTime.ParseExact(value, mapping.DateFormat, culture);
            else if (headerName.Equals(mapping.BookingEndDateColumn, StringComparison.OrdinalIgnoreCase))
                data.BookingEndDate = DateTime.ParseExact(value, mapping.DateFormat, culture);
            else if (headerName.Equals(mapping.PolicyColumn, StringComparison.OrdinalIgnoreCase))
                data.Policy = value;
            else if (headerName.Equals(mapping.CurrencyColumn, StringComparison.OrdinalIgnoreCase))
                data.Currency = value;
            else if (headerName.Equals(mapping.AmountColumn, StringComparison.OrdinalIgnoreCase))
                data.Amount = decimal.Parse(value, culture);
            else if (headerName.Equals(mapping.ExchangeRateColumn, StringComparison.OrdinalIgnoreCase))
                data.ExchangeRate = decimal.Parse(value, culture);
            else if (headerName.Equals(mapping.ParticipantsColumn, StringComparison.OrdinalIgnoreCase))
                data.Participants = value;
            else if (headerName.Equals(mapping.DocumentUrlColumn, StringComparison.OrdinalIgnoreCase))
                data.DocumentUrl = value;
            else if (headerName.Equals(mapping.NotesColumn, StringComparison.OrdinalIgnoreCase))
                data.Notes = value;
        }
        catch (Exception ex)
        {
            data.ValidationErrors.Add($"Error parsing {headerName}: {ex.Message}");
        }
    }

    private List<string> ParseCsvLine(string line, char delimiter)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Double quote - add single quote
                    current.Append('"');
                    i++; // Skip next quote
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        values.Add(current.ToString());
        return values;
    }

    private string ApplyTransformationRules(string? policy, List<TransformationRule> rules)
    {
        if (string.IsNullOrWhiteSpace(policy))
            return "Uncategorized";

        var activeRules = rules.Where(r => r.IsActive).OrderByDescending(r => r.Priority);

        foreach (var rule in activeRules)
        {
            bool matches = rule.IsRegex
                ? Regex.IsMatch(policy, rule.PolicyPattern, RegexOptions.IgnoreCase)
                : policy.Contains(rule.PolicyPattern, StringComparison.OrdinalIgnoreCase);

            if (matches)
            {
                _logger.LogDebug("Policy '{Policy}' matched rule '{Pattern}' -> Category '{Category}'", 
                    policy, rule.PolicyPattern, rule.CategoryName);
                return rule.CategoryName;
            }
        }

        return "Uncategorized";
    }

    private void ValidateImportedData(ImportedTransactionData data)
    {
        if (string.IsNullOrWhiteSpace(data.TransactionId))
            data.ValidationErrors.Add("Transaction ID is required");

        if (string.IsNullOrWhiteSpace(data.Email))
            data.ValidationErrors.Add("Email is required");

        if (!IsValidEmail(data.Email))
            data.ValidationErrors.Add("Invalid email format");

        if (data.TransactionDate == default)
            data.ValidationErrors.Add("Transaction date is required");

        if (data.Amount == 0)
            data.ValidationErrors.Add("Amount cannot be zero");

        if (string.IsNullOrWhiteSpace(data.Currency))
            data.ValidationErrors.Add("Currency is required");

        if (data.Currency.Length != 3)
            data.ValidationErrors.Add("Currency must be 3-letter code");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async Task<Source> GetOrCreateSourceAsync(string sourceName)
    {
        var source = await _context.Sources.FirstOrDefaultAsync(s => s.Name == sourceName);
        
        if (source == null)
        {
            source = new Source { Name = sourceName };
            _context.Sources.Add(source);
            await _context.SaveChangesAsync();
        }

        return source;
    }

    private async Task<Transaction> CreateTransactionFromImportData(ImportedTransactionData data, Source source)
    {
        var category = await GetOrCreateCategoryAsync(data.DeterminedCategory);

        var transaction = new Transaction
        {
            TransactionId = data.TransactionId,
            SourceId = source.SourceId,
            Email = data.Email,
            TransactionDate = data.TransactionDate,
            AuthorizationDate = data.AuthorizationDate,
            TransactionType = data.TransactionType,
            CategoryId = category.CategoryId,
            Vendor = data.Vendor,
            MerchantCategory = data.MerchantCategory,
            Address = data.Address,
            SourceTripId = data.SourceTripId,
            BookingId = data.BookingId,
            BookingStartDate = data.BookingStartDate,
            BookingEndDate = data.BookingEndDate,
            Policy = data.Policy,
            Currency = data.Currency,
            Amount = data.Amount,
            AmountUSD = data.AmountUSD,
            ExchangeRate = data.ExchangeRate,
            Participants = data.Participants,
            DocumentUrl = data.DocumentUrl,
            Notes = data.Notes,
            DataValidation = true, // Needs validation
            ParticipantsValidated = false,
            IsValid = false,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        return transaction;
    }

    private async Task<Category> GetOrCreateCategoryAsync(string categoryName)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
        
        if (category == null)
        {
            category = new Category { Name = categoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        return category;
    }

    private static CsvFieldMapping GetNavanFieldMapping()
    {
        return new CsvFieldMapping
        {
            TransactionIdColumn = "Transaction ID",
            EmailColumn = "Employee Email",
            TransactionDateColumn = "Transaction Date",
            AuthorizationDateColumn = "Authorization Date",
            TransactionTypeColumn = "Transaction Type",
            VendorColumn = "Vendor",
            MerchantCategoryColumn = "Merchant Category",
            AddressColumn = "Address",
            SourceTripIdColumn = "Trip ID",
            BookingIdColumn = "Booking ID",
            BookingStartDateColumn = "Booking Start Date",
            BookingEndDateColumn = "Booking End Date",
            PolicyColumn = "Policy",
            CurrencyColumn = "Currency",
            AmountColumn = "Amount",
            ExchangeRateColumn = "Exchange Rate",
            ParticipantsColumn = "Participants",
            DocumentUrlColumn = "Receipt URL",
            NotesColumn = "Notes",
            DateFormat = "yyyy-MM-dd",
            CultureInfo = "en-US",
            Delimiter = ',',
            HasHeader = true
        };
    }

    private static CsvFieldMapping GetAgentFieldMapping()
    {
        return new CsvFieldMapping
        {
            TransactionIdColumn = "TransactionID",
            EmailColumn = "Email",
            TransactionDateColumn = "Date",
            VendorColumn = "Merchant",
            AddressColumn = "Location",
            PolicyColumn = "Category",
            CurrencyColumn = "Currency",
            AmountColumn = "Amount",
            DocumentUrlColumn = "Receipt",
            DateFormat = "dd/MM/yyyy",
            CultureInfo = "en-US",
            Delimiter = ',',
            HasHeader = true
        };
    }

    private static CsvFieldMapping GetManualFieldMapping()
    {
        return new CsvFieldMapping
        {
            TransactionIdColumn = "ID",
            EmailColumn = "Email",
            TransactionDateColumn = "Date",
            VendorColumn = "Vendor",
            PolicyColumn = "Category",
            CurrencyColumn = "Currency",
            AmountColumn = "Amount",
            DateFormat = "dd/MM/yyyy",
            CultureInfo = "en-US",
            Delimiter = ',',
            HasHeader = true
        };
    }

    private static List<TransformationRuleDto> GetDefaultTransformationRules()
    {
        return new List<TransformationRuleDto>
        {
            new() { RuleId = 1, PolicyPattern = "tripactions_fees", CategoryName = "Trip fee", Priority = 100 },
            new() { RuleId = 2, PolicyPattern = "Airalo", CategoryName = "Communication", Priority = 90 },
            new() { RuleId = 3, PolicyPattern = "public transport, tolls & parking", CategoryName = "Transportation", Priority = 80 },
            new() { RuleId = 4, PolicyPattern = "Taxi & rideshare", CategoryName = "Transportation", Priority = 80 },
            new() { RuleId = 5, PolicyPattern = "Rental cars", CategoryName = "Transportation", Priority = 80 },
            new() { RuleId = 6, PolicyPattern = "Train travel", CategoryName = "Transportation", Priority = 80 },
            new() { RuleId = 7, PolicyPattern = "Fuel", CategoryName = "Transportation", Priority = 80 },
            new() { RuleId = 8, PolicyPattern = "entertaining clients", CategoryName = "Client entertainment", Priority = 70 },
            new() { RuleId = 9, PolicyPattern = "team events & meals", CategoryName = "Meals", Priority = 70 },
            new() { RuleId = 10, PolicyPattern = "Meals for myself", CategoryName = "Meals", Priority = 70 },
            new() { RuleId = 11, PolicyPattern = "Airfare", CategoryName = "Airfare", Priority = 80 },
            new() { RuleId = 12, PolicyPattern = "Internet access", CategoryName = "Communication", Priority = 60 },
            new() { RuleId = 13, PolicyPattern = "telecommunication_services", CategoryName = "Communication", Priority = 60 },
            new() { RuleId = 14, PolicyPattern = "Lodging", CategoryName = "Lodging", Priority = 80 },
            new() { RuleId = 15, PolicyPattern = "Software", CategoryName = "Other", Priority = 50 },
            new() { RuleId = 16, PolicyPattern = "Conference attendance", CategoryName = "Other", Priority = 50 }
        };
    }
}