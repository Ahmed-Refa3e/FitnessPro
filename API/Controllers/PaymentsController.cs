using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount, // Amount in cents (2000 = $20.00)
            Currency = request.Currency, // e.g. "usd"
            PaymentMethodTypes = ["card"],
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }
}
public class CreatePaymentIntentRequest
{
    public long Amount { get; set; } // Amount in cents
    public string Currency { get; set; } = "usd"; // Default to USD
}

