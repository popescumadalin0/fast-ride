using System.Collections.Generic;

namespace FastRide.Client.Models;

public class GoogleLocationAddressComponent
{
    // ReSharper disable once InconsistentNaming
    public List<string> types { get; set; }

    // ReSharper disable once InconsistentNaming
    public string long_name { get; set; }
}