using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Services.Contracts;

public interface IDistanceService
{
    double CalculatePricePerDistance(double distanceInKm, double durationInMinutes);

    double GetDistanceBetweenLocations(Geolocation locationA, Geolocation locationB);

    double EstimateTimeInHours(double distanceKm, double averageSpeedKmH);
}