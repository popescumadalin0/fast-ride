namespace FastRide.Server.Contracts.Constants;

/// event pattern: {source}.{action}
public class SignalRConstants
{
    public const string HubName = "api";

    public const string ClientJoinUserToGroup = "client.join-user-group";
    
    public const string ClientLeaveUserFromGroup = "client.leave-user-group";

    public const string ClientCreateNewRide = "client.create-new-ride";

    public const string ServerCreateNewRide = "server.create-new-ride";

    public const string ClientAcceptRide = "client.accept-ride";
    
    public const string ServerAcceptRide = "server.accept-ride";
    
    public const string ServerSendPriceCalculation = "server.send-price-calculation";
    
    public const string ClientSendPriceCalculation = "client.send-price-calculation";
    
    public const string ServerSendPriceCalculationResponseReceived = "server.send-price-calculation-response-received";

    
    /*public const string SendPaymentRequest = "SendPaymentRequest";
    
    public const string SendPaymentResponse = "SendPaymentResponse";*/
    
    public const string ClientNotifyUserGeolocation = "client.notify-user-geolocation";
    
    public const string ServerNotifyUserGeolocation = "server.notify-user-geolocation";

}