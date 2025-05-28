using System.ComponentModel;

namespace FastRide.Server.Contracts.Enums;

public enum CompleteStatus
{
    [Description("Driver not found!")]
    DriverNotFound = 0,

    [Description("Completed!")] Completed,

    [Description("Payment refused!")]
    PaymentRefused,
    
    [Description("Cancelled!")]
    Cancelled,
}