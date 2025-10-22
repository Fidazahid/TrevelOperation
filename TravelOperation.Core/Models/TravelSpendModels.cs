namespace TravelOperation.Core.Models;

public class TravelSpendReportItem
{
    public int TripId { get; set; }
    public string TripName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public string Countries { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmountUSD { get; set; }
    public decimal CostPerDay { get; set; }
    public decimal AirfareUSD { get; set; }
    public string CabinClasses { get; set; } = string.Empty;
    public decimal LodgingUSD { get; set; }
    public decimal LodgingPerNight { get; set; }
    public decimal MealsUSD { get; set; }
    public decimal MealsPerDay { get; set; }
    public decimal TransportationUSD { get; set; }
    public decimal ClientEntertainmentUSD { get; set; }
    public decimal CommunicationUSD { get; set; }
    public decimal OtherUSD { get; set; }
    public decimal TaxExposure { get; set; }
    public string Owner { get; set; } = string.Empty;
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
}

public class TravelSpendSummary
{
    public int TotalTrips { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageCostPerTrip { get; set; }
    public decimal TotalTaxExposure { get; set; }
    public decimal TotalAirfare { get; set; }
    public decimal TotalLodging { get; set; }
    public decimal TotalMeals { get; set; }
    public decimal TotalTransportation { get; set; }
    public decimal TotalClientEntertainment { get; set; }
    public decimal TotalCommunication { get; set; }
    public decimal TotalOther { get; set; }
}