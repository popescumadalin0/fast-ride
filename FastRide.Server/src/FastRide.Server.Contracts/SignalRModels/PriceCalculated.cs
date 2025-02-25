namespace FastRide.Server.Contracts.SignalRModels;

public class PriceCalculated
{
    public string InstanceId { get; set; }
    
    public double Price { get; set; }
}