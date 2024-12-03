namespace FastRide.Client.Contracts;

public interface IDistanceService
{
    decimal CalculatePricePerDistance(decimal distanceInKm, decimal durationInMinutes);
}