namespace FastRide.Server.Services.Contracts;

public interface IDistanceService
{
    decimal CalculatePricePerDistance(decimal distanceInKm, decimal durationInMinutes);
}