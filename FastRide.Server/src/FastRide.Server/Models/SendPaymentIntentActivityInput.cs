namespace FastRide.Server.Models;

public class SendPaymentIntentActivityInput
{
    public string InstanceId { get; set; }
    
    public double Price { get; set; }
    
    public string UserId { get; set; }
}