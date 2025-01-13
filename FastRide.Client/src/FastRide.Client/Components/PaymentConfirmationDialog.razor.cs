using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

//todo: payment confirmation
public partial class PaymentConfirmationDialog : ComponentBase
{
    private string _buttonName = string.Empty;
    private bool _completed;

    private int _index;

    private string _paymentTitle = string.Empty;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _paymentTitle = "Confirm the ride";

        _buttonName = "Go pay";

        StateHasChanged();
    }

    private async Task Submit()
    {
        //OverlayState.DataLoading = true;

        /*await Sender.BookRideAsync(new Server.Contracts.Models.Ride()
        {
            Cost = 1,
            Id = "test",
            Status = RideStatus.Pending,
            DriverEmail = "test@test.com",
            UserEmail = "test@test.com",
        });*/

        /*
         var totalAmount = DistanceService.Calculate...()
        // Obține PaymentIntent din API
        var response = await Http.PostAsJsonAsync("https://yourfunctionapp.azurewebsites.net/api/CreatePaymentIntent", new { Amount = totalAmount });
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

        OverlayState.DataLoading = false;

        if (!response.Success)
        {
            Snackbar.Add(response.ResponseMessage, Severity.Error);
            return;
        }*/

        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog.Cancel();
}