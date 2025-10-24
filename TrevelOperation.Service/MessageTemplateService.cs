using System.Text;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TrevelOperation.Service;

public class MessageTemplateService : IMessageTemplateService
{
    private readonly ISettingsService _settingsService;
    
    // Default company domains - can be extended via configuration
    private readonly HashSet<string> CompanyDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "@company.com",
        "@wsc.com",
        "@subsidiary.com",
        "@walkme.com",
        "@walkmeinc.com"
    };

    public MessageTemplateService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string GenerateMealsMessage(Transaction transaction, List<string>? participants = null)
    {
        var participantAnalysis = AnalyzeParticipants(transaction.Participants, GetDefaultInternalEmails());
        
        var firstName = ExtractFirstName(transaction.Email);
        var message = new StringBuilder();

        message.AppendLine($"Hi {firstName},");
        message.AppendLine();
        message.AppendLine("I have a question about the following travel-related transaction:");
        message.AppendLine();
        message.AppendLine($"Transaction ID: {transaction.TransactionId}");
        message.AppendLine($"Date: {transaction.TransactionDate:dd/MM/yyyy}");
        message.AppendLine($"Category: Meals");
        message.AppendLine($"Vendor: {transaction.Vendor}");
        message.AppendLine($"Address: {transaction.Address}");
        message.AppendLine($"Amount: {transaction.Currency} {transaction.Amount:N2}");
        
        if (participantAnalysis.HasExternalParticipants || participants?.Any() == true)
        {
            var participantList = participants?.Any() == true ? participants : participantAnalysis.AllParticipants;
            message.AppendLine($"Participants: {string.Join(", ", participantList)}");
        }
        
        message.AppendLine($"Documentation: {transaction.DocumentUrl ?? "Not provided"}");
        message.AppendLine();

        if (participantAnalysis.HasExternalParticipants)
        {
            message.AppendLine($"The system identified the following external participants: {string.Join(", ", participantAnalysis.ExternalParticipants)}, while this transaction was marked as \"Meals\". If this was a meeting with external participants, please share all the participants in the meeting and the purpose.");
        }
        else if (participantAnalysis.HasInternalParticipants)
        {
            message.AppendLine($"The system identified the following participants: {string.Join(", ", participantAnalysis.InternalParticipants)}. Can you please confirm the transaction was shared with them and you paid on their behalf as well?");
            message.AppendLine();
            message.AppendLine("Based on company policy, shared spend among WSC employees is not allowed. Each employee should pay for their own expenses.");
        }
        else
        {
            message.AppendLine("Were there other participants (internal or external) in this meal? If so, could you please provide their email addresses?");
            message.AppendLine();
            message.AppendLine("Based on company policy, shared spend among WSC employees is not allowed. Each employee should pay for their own expenses.");
        }

        message.AppendLine();
        message.AppendLine("Thank you!");

        return message.ToString();
    }

    public string GenerateClientEntertainmentMessage(Transaction transaction, List<string> participants)
    {
        var participantAnalysis = AnalyzeParticipants(string.Join(", ", participants), GetDefaultInternalEmails());
        var firstName = ExtractFirstName(transaction.Email);
        var message = new StringBuilder();

        message.AppendLine($"Hi {firstName},");
        message.AppendLine();
        message.AppendLine("The following transaction was categorized as Client entertainment:");
        message.AppendLine();
        message.AppendLine($"Transaction ID: {transaction.TransactionId}");
        message.AppendLine($"Date: {transaction.TransactionDate:dd/MM/yyyy}");
        message.AppendLine($"Category: Client entertainment");
        message.AppendLine($"Vendor: {transaction.Vendor}");
        message.AppendLine($"Address: {transaction.Address}");
        message.AppendLine($"Amount: {transaction.Currency} {transaction.Amount:N2}");
        message.AppendLine($"Participants: {string.Join(", ", participants)}");
        message.AppendLine($"Documentation: {transaction.DocumentUrl ?? "Not provided"}");
        message.AppendLine();

        if (participantAnalysis.HasExternalParticipants)
        {
            message.AppendLine($"The system identified the following external participants: {string.Join(", ", participantAnalysis.ExternalParticipants)}.");
            message.AppendLine();
        }

        message.AppendLine("To comply with tax reporting requirements, could you please provide the names and email addresses of all participants (both internal WSC employees and external customers/prospects)?");
        message.AppendLine();
        message.AppendLine("This information is required for proper documentation and may be needed in the event of a future tax audit.");
        message.AppendLine();
        message.AppendLine("Thank you!");

        return message.ToString();
    }

    public string GenerateOtherCategoryMessage(Transaction transaction, Trip? trip = null)
    {
        var firstName = ExtractFirstName(transaction.Email);
        var message = new StringBuilder();

        message.AppendLine($"Hi {firstName},");
        message.AppendLine();
        message.AppendLine("I have a question about the following travel-related transaction:");
        message.AppendLine();
        message.AppendLine($"Transaction ID: {transaction.TransactionId}");
        message.AppendLine($"Date: {transaction.TransactionDate:dd/MM/yyyy}");
        
        if (trip != null)
        {
            message.AppendLine($"Trip: {trip.TripName}, {trip.StartDate:dd/MM/yyyy} - {trip.EndDate:dd/MM/yyyy}");
        }
        
        message.AppendLine($"Category: Other");
        message.AppendLine($"Vendor: {transaction.Vendor}");
        message.AppendLine($"Address: {transaction.Address}");
        message.AppendLine($"Amount: {transaction.Currency} {transaction.Amount:N2}");
        message.AppendLine($"Documentation: {transaction.DocumentUrl ?? "Not provided"}");
        message.AppendLine();
        message.AppendLine("The system wasn't able to categorize this transaction and we need to select a proper category for tax purposes.");
        message.AppendLine();
        message.AppendLine("What is the nature of this transaction?");
        message.AppendLine();
        message.AppendLine("Thank you!");

        return message.ToString();
    }

    public string GenerateDocumentationMessage(Transaction transaction, Priority priority = Priority.Normal)
    {
        var firstName = ExtractFirstName(transaction.Email);
        var message = new StringBuilder();
        var urgencyText = GetUrgencyText(priority);

        message.AppendLine($"Hi {firstName},");
        message.AppendLine();
        message.AppendLine($"{urgencyText}We need documentation for the following transaction:");
        message.AppendLine();
        message.AppendLine($"Transaction ID: {transaction.TransactionId}");
        message.AppendLine($"Date: {transaction.TransactionDate:dd/MM/yyyy}");
        message.AppendLine($"Category: {transaction.Category?.Name ?? "Unknown"}");
        message.AppendLine($"Vendor: {transaction.Vendor}");
        message.AppendLine($"Address: {transaction.Address}");
        message.AppendLine($"Amount: {transaction.Currency} {transaction.Amount:N2}");
        message.AppendLine();
        
        switch (priority)
        {
            case Priority.Critical:
                message.AppendLine("URGENT: Please provide the receipt/documentation for this transaction immediately as it is required for tax compliance.");
                break;
            case Priority.High:
                message.AppendLine("Please provide the receipt/documentation for this transaction as soon as possible.");
                break;
            default:
                message.AppendLine("Please provide the receipt/documentation for this transaction when convenient.");
                break;
        }

        message.AppendLine();
        message.AppendLine("You can upload the documentation through the travel expense system or email it directly.");
        message.AppendLine();
        message.AppendLine("Thank you!");

        return message.ToString();
    }

    public List<string> DetectExternalParticipants(string? participantsString, List<string> internalEmails)
    {
        return AnalyzeParticipants(participantsString, internalEmails).ExternalParticipants;
    }

    public List<string> DetectInternalParticipants(string? participantsString, List<string> internalEmails)
    {
        return AnalyzeParticipants(participantsString, internalEmails).InternalParticipants;
    }

    public ParticipantAnalysis AnalyzeParticipants(string? participantsString, List<string> internalEmails)
    {
        var analysis = new ParticipantAnalysis();

        if (string.IsNullOrWhiteSpace(participantsString))
        {
            return analysis;
        }

        // Split by common separators
        var participants = participantsString
            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        analysis.AllParticipants = participants;

        // Classify participants
        foreach (var participant in participants)
        {
            if (IsInternalParticipant(participant, internalEmails))
            {
                analysis.InternalParticipants.Add(participant);
            }
            else
            {
                analysis.ExternalParticipants.Add(participant);
            }
        }

        return analysis;
    }

    public void AddCompanyDomain(string domain)
    {
        if (!string.IsNullOrWhiteSpace(domain))
        {
            // Ensure domain starts with @
            var normalizedDomain = domain.StartsWith("@") ? domain : $"@{domain}";
            CompanyDomains.Add(normalizedDomain);
        }
    }

    public void AddCompanyDomains(IEnumerable<string> domains)
    {
        foreach (var domain in domains)
        {
            AddCompanyDomain(domain);
        }
    }

    public IReadOnlyCollection<string> GetCompanyDomains()
    {
        return CompanyDomains.ToList().AsReadOnly();
    }

    public bool IsCompanyEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            return false;
        }

        return CompanyDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsInternalParticipant(string participant, List<string> internalEmails)
    {
        // Check if exact match in internal emails list
        if (internalEmails.Contains(participant, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check if participant contains company domain
        if (participant.Contains("@", StringComparison.OrdinalIgnoreCase))
        {
            return CompanyDomains.Any(domain => participant.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    private string ExtractFirstName(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return "there";
        }

        // Extract name from email (before @)
        var namePart = email.Split('@')[0];
        
        // Handle common patterns like "first.last" or "first_last"
        var parts = namePart.Split(new[] { '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length > 0)
        {
            // Capitalize first letter
            var firstName = parts[0];
            return char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();
        }

        return "there";
    }

    private string GetUrgencyText(Priority priority)
    {
        return priority switch
        {
            Priority.Critical => "🚨 URGENT: ",
            Priority.High => "⚠️ IMPORTANT: ",
            Priority.Low => "",
            _ => ""
        };
    }

    private async Task<List<string>> GetInternalEmailsAsync()
    {
        try
        {
            // Get all employee emails from Headcount table
            var headcount = await _settingsService.GetAllHeadcountAsync();
            return headcount
                .Where(h => !string.IsNullOrWhiteSpace(h.Email))
                .Select(h => h.Email)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            // Fallback to empty list if headcount service fails
            return new List<string>();
        }
    }

    private List<string> GetDefaultInternalEmails()
    {
        // Synchronous wrapper for backward compatibility
        // In production, prefer async methods
        return GetInternalEmailsAsync().GetAwaiter().GetResult();
    }
}