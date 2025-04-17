using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Payment;

public class PaymentsController : BaseApiController
{
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        //TODO make the amount come from db and the traineeID from the token
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount, // Amount in cents (2000 = $20.00) 
            PaymentMethodTypes = ["card"],
            Currency = "usd",
            Metadata = new Dictionary<string, string>
        {
            { "traineeId", request.TraineeId },
            { "onlineTrainingId", request.OnlineTrainingId.ToString()},
            { "subscriptionType", "online" }
        }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }
}
public class CreatePaymentIntentRequest
{
    [Required]
    public long Amount { get; set; }
    [Required]
    public string TraineeId { get; set; } = null!;
    [Required]
    public int OnlineTrainingId { get; set; }
}


