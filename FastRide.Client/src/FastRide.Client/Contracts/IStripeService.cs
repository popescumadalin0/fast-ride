using System;
using System.Threading.Tasks;

namespace FastRide.Client.Contracts;

public interface IStripeService
{
    event Func<bool, Task> OnValidationChanged;

    ValueTask StripeInitializeAsync(string clientSecret, string publishKey);

    ValueTask<string> StripeCheckoutAsync();
}