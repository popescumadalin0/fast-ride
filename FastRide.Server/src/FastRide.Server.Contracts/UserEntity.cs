namespace FastRide.Server.Contracts;

public class User
{
    public string Email { get; set; }

    public string NameIdentifier { get; set; }

    public UserType UserType { get; set; }
    
    public double Rating { get; set; }
    
    public string PictureUrl { get; set; }
    
    public string PhoneNumber { get; set; }
}