using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class SendPaymentIntentActivityInput
{
    public string InstanceId { get; set; }
    
    public decimal Price { get; set; }
    
    public string UserId { get; set; }
}