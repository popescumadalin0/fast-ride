namespace FastRide.Client.Models;

public class OpenStreetMapResponse
{
    public AddressModel Address { get; set; }

    // ReSharper disable once InconsistentNaming
    public string display_name { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public double lat { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public double lon { get; set; }
}