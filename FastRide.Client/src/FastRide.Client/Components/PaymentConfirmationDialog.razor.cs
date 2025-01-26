using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class PaymentConfirmationDialog : ComponentBase, IDisposable
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }
    
    [Inject] private IStripeService StripeService { get; set; }
    
    [Inject] private IConfiguration Configuration { get; set; }
    
    private bool _completed;

    private int _index;
    private bool _nextDisabled = true;

    private decimal _price;
    private bool _stepperLoading = true;
    
    private string _instanceId = string.Empty;

    public void Dispose()
    {
        SignalRService.SendPriceCalculated -= PriceReceivedAsync;
        SignalRService.SendPaymentIntentReceived -= PaymentIntentReceivedAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        SignalRService.SendPriceCalculated += PriceReceivedAsync;
        SignalRService.SendPaymentIntentReceived += PaymentIntentReceivedAsync;

        StateHasChanged();
    }

    private Task PriceReceivedAsync(PriceCalculated arg)
    {
        _nextDisabled = false;
        _stepperLoading = false;
        _price = arg.Price;
        
        _instanceId = arg.InstanceId;

        StateHasChanged();
        
        return Task.CompletedTask;
    }

    private async Task PaymentIntentReceivedAsync(SendPaymentIntent message)
    {
        _stepperLoading = false;
        StateHasChanged();

        await StripeService.StripeInitializeAsync(message.ClientSecret, Configuration["Stripe:PublishKey"]);
        
        _nextDisabled = false;
        StateHasChanged();

    }

    private async Task NextStepperAsync(MudStepper stepper)
    {
        switch (stepper.ActiveIndex)
        {
            case 0:
            {
                await SignalRService.ConfirmPriceCalculatedAsync(_instanceId, _price);
                break;
            }
            case 1:
            {
                await StripeService.StripeCheckoutAsync();
                break;
            }
        }
        await stepper.NextStepAsync();
        _nextDisabled = true;
        _stepperLoading = true;
    }

    private async Task CancelAsync(MudStepper stepper)
    {
        switch (stepper.ActiveIndex)
        {
            case 0:
            {
                await SignalRService.ConfirmPriceCalculatedAsync(_instanceId, 0);
                break;
            }
            case 1:
            {
                await SignalRService.ConfirmPaymentAsync(_instanceId, false);
                break;
            }
        }
        MudDialog.Cancel();
    }
}