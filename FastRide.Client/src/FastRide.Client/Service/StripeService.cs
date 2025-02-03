using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class StripeService : IStripeService
{
    private readonly DotNetObjectReference<StripeService> _dotNetObjectReference;
    private readonly IJSRuntime _jsRuntime;

    private event Func<string, ValueTask> PaymentConfirmed = default!;

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
        var tcs = new TaskCompletionSource<string>();

        Func<string, ValueTask> paymentConfirmed = null;
        paymentConfirmed = (message) =>
        {
            tcs.SetResult(message);

            PaymentConfirmed -= paymentConfirmed;

            return ValueTask.CompletedTask;
        };

        PaymentConfirmed += paymentConfirmed;

        await _jsRuntime.InvokeAsync<string>("window.checkoutStripe", _dotNetObjectReference);

        var message = await tcs.Task;

        return message;
    }

    [JSInvokable]
    public Task OnValidationCallback(bool isValid)
    {
        OnValidationChanged?.Invoke(isValid);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnPaymentChangedAsync(string message)
    {
        if (PaymentConfirmed != null!)
        {
            await PaymentConfirmed.Invoke(message);
        }
    }
}