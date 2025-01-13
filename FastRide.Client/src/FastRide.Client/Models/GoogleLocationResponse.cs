using System.Collections.Generic;

namespace FastRide.Client.Models;

public class GoogleLocationResponse
{
    // ReSharper disable once InconsistentNaming
    public List<GoogleLocationResult> results { get; set; }
}