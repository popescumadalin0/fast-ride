using System;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Services.Services;

public class DistanceService(ILogger<DistanceService> logger)
    : IDistanceService
{
    private readonly double _basePrice = double.Parse(Environment.GetEnvironmentVariable("Stripe:BasePrice")!);

    private readonly double _pricePerKm = double.Parse(Environment.GetEnvironmentVariable("Stripe:PricePerKm")!);

    private const double EarthRadiusKm = 6371;

    private readonly double _pricePerMinute =
        double.Parse(Environment.GetEnvironmentVariable("Stripe:PricePerMinute")!);

    public double CalculatePricePerDistance(double distanceInKm, double durationInMinutes)
    {
        logger.LogInformation("Calculate price for current distance!");
        return _basePrice + (_pricePerKm * distanceInKm) + (_pricePerMinute * durationInMinutes);
    }

    public double GetDistanceBetweenLocations(Geolocation locationA, Geolocation locationB)
    {
        var dLat = ToRadians(locationB.Latitude - locationA.Latitude);
        var dLon = ToRadians(locationB.Longitude - locationA.Longitude);
        locationA.Latitude = ToRadians(locationA.Latitude);
        locationB.Latitude = ToRadians(locationB.Latitude);

        var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(locationA.Latitude) * Math.Cos(locationB.Latitude) * Math.Pow(Math.Sin(dLon / 2), 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    public double EstimateTimeInHours(double distanceKm, double averageSpeedKmH)
    {
        return distanceKm / averageSpeedKmH;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
}