using System.Threading.Tasks;

namespace FastRide.Client.Contracts;

public interface IStripeService
{
    ValueTask StripeInitializeAsync(string clientSecret, string publishKey);
    ValueTask StripeCheckoutAsync();
}