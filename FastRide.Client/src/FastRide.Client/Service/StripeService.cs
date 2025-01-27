using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class StripeService : IStripeService
{
    private readonly DotNetObjectReference<StripeService> _dotNetObjectReference;
    private readonly IJSRuntime _jsRuntime;

    public StripeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;

        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public event Func<bool, Task> OnValidationChanged;

    public async ValueTask StripeInitializeAsync(string clientSecret, string publishKey)
    {
        await _jsRuntime.InvokeVoidAsync("window.initializeStripe",
            _dotNetObjectReference,
            clientSecret,
            publishKey,
            nameof(OnValidationCallback));
    }

    public async ValueTask<string> StripeCheckoutAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("window.checkoutStripe");
    }

    [JSInvokable]
    public Task OnValidationCallback(bool isValid)
    {
        OnValidationChanged?.Invoke(isValid);
        return Task.CompletedTask;
    }
}