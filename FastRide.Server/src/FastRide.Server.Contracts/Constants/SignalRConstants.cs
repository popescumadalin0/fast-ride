namespace FastRide.Server.Contracts.Constants;

/// event pattern: {source}.{action}
public class SignalRConstants
{
    public const string HubName = "api";

    public const string ClientJoinUserToGroup = "client.join-user-group";

    public const string ClientLeaveUserFromGroup = "client.leave-user-group";

    public const string ClientCreateNewRide = "client.create-new-ride";

    public const string ServerCreateNewRide = "server.create-new-ride";

    public const string ClientDriverAcceptRide = "client.driver-accept-ride";

    public const string ServerDriverRideAccepted = "server.driver-ride-accepted";

    public const string ServerDriverAcceptRide = "server.driver-accept-ride";

    public const string ServerSendPriceCalculation = "server.send-price-calculation";

    public const string ClientSendPriceCalculation = "client.send-price-calculation";

    public const string ServerSendPaymentIntent = "server.send-payment-intent";

    public const string ClientSendPaymentIntent = "client.send-payment-intent";

    public const string ClientNotifyUserGeolocation = "client.notify-user-geolocation";

    public const string ServerNotifyUserGeolocation = "server.notify-user-geolocation";

    public const string ServerNotifyState = "server.notify-state";

    public const string ServerNotifyDriverTimeout = "server.notify-driver-timeout";

    public const string ServerCancelRide = "server.cancel-ride";

    public const string ClientCancelRide = "client.cancel-ride";

    public const string ClientDriverArrived = "client.driver-arrived";

    public const string ServerSendRatingRequest = "server.send-rating-request";

    public const string ClientSendRatingRequest = "client.send-rating-request";
}