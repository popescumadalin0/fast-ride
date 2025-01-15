using System;
using FastRide.Server.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Services.Services;

public class DistanceService(ILogger<DistanceService> logger)
    : IDistanceService
{
    private readonly decimal _basePrice = decimal.Parse(Environment.GetEnvironmentVariable("Stripe:BasePrice")!);

    private readonly decimal _pricePerKm = decimal.Parse(Environment.GetEnvironmentVariable("Stripe:PricePerKm")!);

    private readonly decimal _pricePerMinute =
        decimal.Parse(Environment.GetEnvironmentVariable("Stripe:PricePerMinute")!);

    public decimal CalculatePricePerDistance(decimal distanceInKm, decimal durationInMinutes)
    {
        logger.LogInformation("Calculate price for current distance!");
        return _basePrice + (_pricePerKm * distanceInKm) + (_pricePerMinute * durationInMinutes);
    }
}