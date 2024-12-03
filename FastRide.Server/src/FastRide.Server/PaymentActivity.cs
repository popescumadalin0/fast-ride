using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server;

public class PaymentActivity
{
    private readonly ILogger<PaymentActivity> _logger;

    public PaymentActivity(ILogger<PaymentActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(PaymentActivity))]
    public Task RunAsync([ActivityTrigger] string name)
    {
        _logger.LogInformation("Saying hello to {name}.", name);
        //todo
        /*
         var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(requestBody);

    var paymentIntentService = new PaymentIntentService();
    var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
    {
        Amount = (long)(paymentRequest.Amount * 100), // Amount in cents
        Currency = "ron",
        PaymentMethodTypes = new List<string> { "card" },
    });

    return new OkObjectResult(new { PaymentIntentId = paymentIntent.Id, ClientSecret = paymentIntent.ClientSecret });
         */

        return Task.CompletedTask;
    }
}