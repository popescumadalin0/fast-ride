namespace FastRide.Server.Contracts.SignalRModels;

public class SendPaymentIntent
{
    public string InstanceId { get; set; }
    
    public string ClientSecret { get; set; }
}