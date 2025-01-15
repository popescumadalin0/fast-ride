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

    private decimal _price;
    private bool _nextDisabled = true;
    private bool _stepperLoading = true;


    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SignalRService.SendPriceCalculated += PriceReceivedAsync;

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

    private async Task PaymentIntentReceivedAsync()
    {
        //OverlayState.DataLoading = true;

        // Obține PaymentIntent din API
        var response = await Http.PostAsJsonAsync("https://yourfunctionapp.azurewebsites.net/api/CreatePaymentIntent",
            new { Amount = totalAmount });

        var paymentData = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Stripe.js
        var stripe = Stripe("your-stripe-publishable-key");
        var elements = stripe.elements();
        var cardElement = elements.create("card"); 
        cardElement.mount("#card-element");

        var result = await stripe.confirmCardPayment(paymentData.ClientSecret, new
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

        //OverlayState.DataLoading = false;

        if (!response.Success)
        {
            Snackbar.Add(response.ResponseMessage, Severity.Error);
            return;
        }

        MudDialog.Close(DialogResult.Ok(true));
    }

    private async Task NextStepperAsync(MudStepper stepper)
    {
        await stepper.NextStepAsync();
        _nextDisabled = true;
        _stepperLoading = true;
    }

    private void Cancel() => MudDialog.Cancel();

    public void Dispose()
    {
        SignalRService.SendPriceCalculated -= PriceReceivedAsync;
    }
}