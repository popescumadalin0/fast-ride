using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class StripeService : IStripeService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<StripeService> _dotNetObjectReference;

    public StripeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;

        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public async ValueTask StripeInitializeAsync(string clientSecret, string publishKey)
    {
        await _jsRuntime.InvokeVoidAsync("window.initializeStripe",
            clientSecret,
            publishKey);
    }

    public async ValueTask StripeCheckoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("window.checkoutStripe");
    }
}