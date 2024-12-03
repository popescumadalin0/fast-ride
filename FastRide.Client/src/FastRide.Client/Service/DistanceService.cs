using System;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace FastRide.Client.Service;

public class DistanceService : IDistanceService
{
    private readonly ILogger<DistanceService> _logger;

    private readonly decimal _pricePerKm;
    private readonly decimal _pricePerMinute;
    private readonly decimal _basePrice;
    
    public DistanceService(IConfiguration configuration, ILogger<DistanceService> logger)
    {
        _logger = logger;
        
        _pricePerKm = decimal.Parse(configuration["Stripe:PricePerKm"]!);
        _pricePerMinute = decimal.Parse(configuration["Stripe:PricePerMinute"]!);
        _basePrice = decimal.Parse(configuration["Stripe:BasePrice"]!);
    }
    
    public decimal CalculatePricePerDistance(decimal distanceInKm, decimal durationInMinutes)
    {
        _logger.LogInformation("Calculate price for current distance!");
        return _basePrice + (_pricePerKm * distanceInKm) + (_pricePerMinute * durationInMinutes);
    }
}