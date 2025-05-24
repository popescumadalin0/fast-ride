using System;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Services.Services;

public class DistanceService(ILogger<DistanceService> logger)
    : IDistanceService
{
    private readonly double _basePrice = double.Parse(Environment.GetEnvironmentVariable("Distance:BasePrice")!);

    private readonly double _pricePerKm = double.Parse(Environment.GetEnvironmentVariable("Distance:PricePerKm")!);

    private const double EarthRadiusKm = 6371;

    private readonly double _pricePerMinute =
        double.Parse(Environment.GetEnvironmentVariable("Distance:PricePerMinute")!);

    public double CalculatePricePerDistance(double distanceInKm, double durationInMinutes)
    {
        logger.LogInformation("Calculate price for current distance!");
        return _basePrice + _pricePerKm * distanceInKm + _pricePerMinute * durationInMinutes;
    }

    public double GetDistanceBetweenLocations(Geolocation locationA, Geolocation locationB)
    {
        var lat1Rad = ToRadians(locationA.Latitude);
        var lon1Rad = ToRadians(locationA.Longitude);
        var lat2Rad = ToRadians(locationB.Latitude);
        var lon2Rad = ToRadians(locationB.Longitude);

        var dLat = lat2Rad - lat1Rad;
        var dLon = lon2Rad - lon1Rad;

        var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Pow(Math.Sin(dLon / 2), 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    public double EstimateTimeInHours(double distanceKm, double averageSpeedKmH)
    {
        return distanceKm / averageSpeedKmH;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
}