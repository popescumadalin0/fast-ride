using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class PaymentConfirmationDialog : ComponentBase, IDisposable
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }
    
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
        /*var stripe = Stripe("your-stripe-publishable-key");
        var elements = stripe.elements();
        var cardElement = elements.create("card");
        cardElement.mount("#card-element");

        var result = await stripe.confirmCardPayment(message.ClientSecret, new
        {
            payment_method = new
            {
                card = cardElement
            }
        });

        if (result.paymentIntent.status == "succeeded")
        {
            // Plată reușită
        }
        else
        {
            //Snackbar.Add(result.ResponseMessage, Severity.Error);
        }*/
    }

    private async Task NextStepperAsync(MudStepper stepper)
    {
        switch (stepper.ActiveIndex)
        {
            case 0:
            {
                await SignalRService.ConfirmPriceCalculated(_instanceId, true);
                break;
            }
            case 1:
            {
                
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
                await SignalRService.ConfirmPriceCalculated(_instanceId, false);
                break;
            }
            case 1:
            {
                
                break;
            }
        }
        MudDialog.Cancel();
    }
}