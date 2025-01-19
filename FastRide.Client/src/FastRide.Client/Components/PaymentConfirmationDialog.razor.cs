using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

//todo: payment confirmation
public partial class PaymentConfirmationDialog : ComponentBase, IDisposable
{
    private bool _completed;

    private int _index;
    private bool _nextDisabled = true;

    private decimal _price;
    private bool _stepperLoading = true;


    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

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
        StateHasChanged();

        return Task.CompletedTask;
    }

    private Task PriceCalculatedResponseReceivedAsync()
    {
        _nextDisabled = false;

        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task PaymentIntentReceivedAsync(SendPaymentIntent message)
    {
        var stripe = Stripe("your-stripe-publishable-key");
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
        }
    }

    private async Task NextStepperAsync(MudStepper stepper)
    {
        //todo: get current step and make a switch to trigger the client response
        await stepper.NextStepAsync();
        _nextDisabled = true;
        _stepperLoading = true;
    }

    private void Cancel() => MudDialog.Cancel();
}