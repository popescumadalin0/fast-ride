using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;
//todo: payment confirmation
public partial class PaymentConfirmationDialog : ComponentBase
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private IDistanceService  DistanceService { get; set; }

    [Inject] private ISignalRSender SignalRSender { get; set; }
    
    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    private string _phoneNumber;

    protected override async Task OnInitializedAsync()
    {
        OverlayState.DataLoading = true;
        var userInformation = await FastRideApiClient.GetCurrentUserAsync();
        
        /*OverlayState.DataLoading = false;
        
        if (!userInformation.Success)
        {
            Snackbar.Add(userInformation.ResponseMessage, Severity.Error);
            return;
        }

        _phoneNumber = userInformation.Response.PhoneNumber;*/
        
        StateHasChanged();
    }

    private async Task Submit()
    {
        OverlayState.DataLoading = true;

        await SignalRSender.BookRideAsync(new Server.Contracts.Ride()
        {
            Cost = 1,
            Id = "test",
            Status = RideStatus.Pending,
            DriverEmail = "test@test.com",
            UserEmail = "test@test.com",
        });

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