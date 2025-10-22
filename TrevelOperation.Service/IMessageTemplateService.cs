using TravelOperation.Core.Models.Entities;

namespace TrevelOperation.Service;

public interface IMessageTemplateService
{
    string GenerateMealsMessage(Transaction transaction, List<string>? participants = null);
    string GenerateClientEntertainmentMessage(Transaction transaction, List<string> participants);
    string GenerateOtherCategoryMessage(Transaction transaction, Trip? trip = null);
    string GenerateDocumentationMessage(Transaction transaction, Priority priority = Priority.Normal);
    
    List<string> DetectExternalParticipants(string? participantsString, List<string> internalEmails);
    List<string> DetectInternalParticipants(string? participantsString, List<string> internalEmails);
    ParticipantAnalysis AnalyzeParticipants(string? participantsString, List<string> internalEmails);
}

public enum Priority
{
    Low,
    Normal,
    High,
    Critical
}

public class ParticipantAnalysis
{
    public List<string> AllParticipants { get; set; } = new();
    public List<string> InternalParticipants { get; set; } = new();
    public List<string> ExternalParticipants { get; set; } = new();
    public bool HasExternalParticipants => ExternalParticipants.Any();
    public bool HasInternalParticipants => InternalParticipants.Any();
    public bool HasNoParticipants => !AllParticipants.Any();
}