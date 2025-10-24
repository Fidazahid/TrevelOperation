using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TravelOperation.Core.Services;

public class ValidationService : IValidationService
{
    private readonly TravelDbContext _context;

    public ValidationService(TravelDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateTransactionAsync(Transaction transaction)
    {
        var result = new ValidationResult
        {
            EntityId = transaction.TransactionId,
            EntityType = "Transaction",
            IsValid = true
        };

        // Rule 1: Transaction ID is required
        if (string.IsNullOrWhiteSpace(transaction.TransactionId))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "TransactionId",
                Message = "Transaction ID is required",
                RuleCode = "TXN_ID_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 2: Email is required and must be valid format
        if (string.IsNullOrWhiteSpace(transaction.Email))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Email",
                Message = "Email is required",
                RuleCode = "TXN_EMAIL_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }
        else if (!IsValidEmail(transaction.Email))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Email",
                Message = "Email format is invalid",
                RuleCode = "TXN_EMAIL_INVALID",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 3: Transaction date is required
        if (transaction.TransactionDate == default)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "TransactionDate",
                Message = "Transaction date is required",
                RuleCode = "TXN_DATE_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 4: Transaction date cannot be in the future
        if (transaction.TransactionDate > DateTime.Now.Date)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "TransactionDate",
                Message = "Transaction date cannot be in the future",
                RuleCode = "TXN_DATE_FUTURE",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 5: Transaction date cannot be more than 2 years old
        if (transaction.TransactionDate < DateTime.Now.Date.AddYears(-2))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "TransactionDate",
                Message = "Transaction is more than 2 years old",
                RuleCode = "TXN_DATE_OLD"
            });
        }

        // Rule 6: Amount must be numeric and not zero
        if (transaction.Amount == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Amount",
                Message = "Amount cannot be zero",
                RuleCode = "TXN_AMOUNT_ZERO",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 7: Currency must be valid 3-letter code
        if (string.IsNullOrWhiteSpace(transaction.Currency) || transaction.Currency.Length != 3)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Currency",
                Message = "Currency must be a valid 3-letter code (e.g., USD, EUR, ILS)",
                RuleCode = "TXN_CURRENCY_INVALID",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 8: Exchange rate should be positive
        if (transaction.ExchangeRate.HasValue && transaction.ExchangeRate.Value <= 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "ExchangeRate",
                Message = "Exchange rate must be positive",
                RuleCode = "TXN_EXCHANGE_RATE_INVALID",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 9: AmountUSD should match Amount * ExchangeRate (if both present)
        if (transaction.ExchangeRate.HasValue && transaction.ExchangeRate.Value > 0)
        {
            var calculatedUSD = transaction.Amount * transaction.ExchangeRate.Value;
            var difference = Math.Abs((transaction.AmountUSD ?? 0) - calculatedUSD);
            
            if (difference > 0.01m) // Allow 1 cent difference for rounding
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Field = "AmountUSD",
                    Message = $"USD amount ({transaction.AmountUSD:F2}) doesn't match calculated value ({calculatedUSD:F2})",
                    RuleCode = "TXN_AMOUNT_USD_MISMATCH"
                });
            }
        }

        // Rule 10: Category is required
        if (transaction.CategoryId == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "CategoryId",
                Message = "Category is required",
                RuleCode = "TXN_CATEGORY_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 11: Vendor is required
        if (string.IsNullOrWhiteSpace(transaction.Vendor))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "Vendor",
                Message = "Vendor name is missing",
                RuleCode = "TXN_VENDOR_MISSING"
            });
        }

        // Rule 12: Document URL should be valid if provided
        if (!string.IsNullOrWhiteSpace(transaction.DocumentUrl) && !IsValidUrl(transaction.DocumentUrl))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "DocumentUrl",
                Message = "Document URL format appears invalid",
                RuleCode = "TXN_DOCUMENT_URL_INVALID"
            });
        }

        // Rule 13: High-value transactions should have documentation
        if (Math.Abs(transaction.AmountUSD ?? transaction.Amount) > 100 && 
            string.IsNullOrWhiteSpace(transaction.DocumentUrl))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "DocumentUrl",
                Message = "High-value transactions (>$100) should have supporting documentation",
                RuleCode = "TXN_HIGH_VALUE_NO_DOC"
            });
        }

        // Rule 14: Participants format validation
        if (!string.IsNullOrWhiteSpace(transaction.Participants))
        {
            var participants = transaction.Participants.Split(',');
            foreach (var participant in participants)
            {
                var email = participant.Trim();
                if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                {
                    result.Warnings.Add(new ValidationWarning
                    {
                        Field = "Participants",
                        Message = $"Participant email '{email}' appears invalid",
                        RuleCode = "TXN_PARTICIPANT_EMAIL_INVALID"
                    });
                }
            }
        }

        // Rule 15: Authorization date should not be after transaction date
        if (transaction.AuthorizationDate.HasValue && 
            transaction.AuthorizationDate.Value > transaction.TransactionDate.AddDays(30))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "AuthorizationDate",
                Message = "Authorization date is more than 30 days after transaction date",
                RuleCode = "TXN_AUTH_DATE_LATE"
            });
        }

        return result;
    }

    public async Task<ValidationResult> ValidateTripAsync(Trip trip)
    {
        var result = new ValidationResult
        {
            EntityId = trip.TripId.ToString(),
            EntityType = "Trip",
            IsValid = true
        };

        // Rule 1: Trip name is required
        if (string.IsNullOrWhiteSpace(trip.TripName))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "TripName",
                Message = "Trip name is required",
                RuleCode = "TRIP_NAME_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 2: Email is required and must be valid
        if (string.IsNullOrWhiteSpace(trip.Email))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Email",
                Message = "Email is required",
                RuleCode = "TRIP_EMAIL_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }
        else if (!IsValidEmail(trip.Email))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Email",
                Message = "Email format is invalid",
                RuleCode = "TRIP_EMAIL_INVALID",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 3: Start date is required
        if (trip.StartDate == default)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "StartDate",
                Message = "Start date is required",
                RuleCode = "TRIP_START_DATE_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 4: End date is required
        if (trip.EndDate == default)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "EndDate",
                Message = "End date is required",
                RuleCode = "TRIP_END_DATE_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 5: End date must be on or after start date
        if (trip.EndDate < trip.StartDate)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "EndDate",
                Message = "End date must be on or after start date",
                RuleCode = "TRIP_END_BEFORE_START",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 6: Duration should match date range
        var expectedDuration = (trip.EndDate - trip.StartDate).Days + 1;
        if (trip.Duration != expectedDuration)
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "Duration",
                Message = $"Duration ({trip.Duration}) doesn't match date range ({expectedDuration} days)",
                RuleCode = "TRIP_DURATION_MISMATCH"
            });
        }

        // Rule 7: Trip duration should be reasonable (1-365 days)
        if (trip.Duration < 1)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Duration",
                Message = "Trip duration must be at least 1 day",
                RuleCode = "TRIP_DURATION_TOO_SHORT",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        if (trip.Duration > 365)
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "Duration",
                Message = "Trip duration exceeds 1 year - please verify",
                RuleCode = "TRIP_DURATION_TOO_LONG"
            });
        }

        // Rule 8: Primary destination is required
        if (string.IsNullOrWhiteSpace(trip.Country1))
        {
            result.Errors.Add(new ValidationError
            {
                Field = "Country1",
                Message = "Primary destination country is required",
                RuleCode = "TRIP_COUNTRY_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        if (string.IsNullOrWhiteSpace(trip.City1))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "City1",
                Message = "Primary destination city is recommended",
                RuleCode = "TRIP_CITY_MISSING"
            });
        }

        // Rule 9: Purpose is required
        if (trip.PurposeId == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "PurposeId",
                Message = "Trip purpose is required",
                RuleCode = "TRIP_PURPOSE_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 10: Trip type is required
        if (trip.TripTypeId == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "TripTypeId",
                Message = "Trip type is required",
                RuleCode = "TRIP_TYPE_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 11: Status is required
        if (trip.StatusId == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "StatusId",
                Message = "Trip status is required",
                RuleCode = "TRIP_STATUS_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 12: Validation status is required
        if (trip.ValidationStatusId == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Field = "ValidationStatusId",
                Message = "Validation status is required",
                RuleCode = "TRIP_VALIDATION_STATUS_REQUIRED",
                Severity = "Error"
            });
            result.IsValid = false;
        }

        // Rule 13: Secondary destination should have both country and city if one is provided
        if (!string.IsNullOrWhiteSpace(trip.Country2) && string.IsNullOrWhiteSpace(trip.City2))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "City2",
                Message = "Secondary destination city is recommended when country is specified",
                RuleCode = "TRIP_CITY2_MISSING"
            });
        }

        if (!string.IsNullOrWhiteSpace(trip.City2) && string.IsNullOrWhiteSpace(trip.Country2))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "Country2",
                Message = "Secondary destination country is recommended when city is specified",
                RuleCode = "TRIP_COUNTRY2_MISSING"
            });
        }

        // Rule 14: Warn about trips in the past without completed status
        if (trip.EndDate < DateTime.Now.Date && trip.StatusId != 4) // Assuming 4 = Completed
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "StatusId",
                Message = "Trip ended but status is not 'Completed'",
                RuleCode = "TRIP_PAST_NOT_COMPLETED"
            });
        }

        // Rule 15: Warn about future trips with completed status
        if (trip.StartDate > DateTime.Now.Date && trip.StatusId == 4) // Assuming 4 = Completed
        {
            result.Warnings.Add(new ValidationWarning
            {
                Field = "StatusId",
                Message = "Trip hasn't started but status is 'Completed'",
                RuleCode = "TRIP_FUTURE_COMPLETED"
            });
        }

        return result;
    }

    public async Task<List<ValidationResult>> ValidateTransactionsAsync(List<Transaction> transactions)
    {
        var results = new List<ValidationResult>();
        
        foreach (var transaction in transactions)
        {
            var result = await ValidateTransactionAsync(transaction);
            results.Add(result);
        }

        return results;
    }

    public async Task<List<ValidationResult>> ValidateTripsAsync(List<Trip> trips)
    {
        var results = new List<ValidationResult>();
        
        foreach (var trip in trips)
        {
            var result = await ValidateTripAsync(trip);
            results.Add(result);
        }

        return results;
    }

    public async Task<List<ValidationRule>> GetValidationRulesAsync()
    {
        // Return configured validation rules
        // In a real implementation, these would be stored in database
        return new List<ValidationRule>
        {
            new ValidationRule
            {
                RuleCode = "TXN_ID_REQUIRED",
                RuleName = "Transaction ID Required",
                EntityType = "Transaction",
                Field = "TransactionId",
                Condition = "NOT NULL",
                Message = "Transaction ID is required",
                IsEnabled = true,
                Severity = "Error"
            },
            new ValidationRule
            {
                RuleCode = "TXN_DATE_FUTURE",
                RuleName = "Transaction Date Not Future",
                EntityType = "Transaction",
                Field = "TransactionDate",
                Condition = "<= TODAY",
                Message = "Transaction date cannot be in the future",
                IsEnabled = true,
                Severity = "Error"
            },
            new ValidationRule
            {
                RuleCode = "TRIP_END_BEFORE_START",
                RuleName = "Trip End Date After Start",
                EntityType = "Trip",
                Field = "EndDate",
                Condition = ">= StartDate",
                Message = "End date must be on or after start date",
                IsEnabled = true,
                Severity = "Error"
            }
            // ... more rules
        };
    }

    public async Task UpdateValidationRulesAsync(List<ValidationRule> rules)
    {
        // In a real implementation, update rules in database
        await Task.CompletedTask;
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

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
