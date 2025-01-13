using System.Collections.Generic;

namespace FastRide.Client.Models;

public class GoogleLocationResult
{
    // ReSharper disable once InconsistentNaming
    public List<GoogleLocationAddressComponent> address_components { get; set; }

    // ReSharper disable once InconsistentNaming
    public string formatted_address { get; set; }
}