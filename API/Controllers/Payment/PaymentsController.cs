using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace API.Controllers.Payment;

public class PaymentsController(
    IOnlineTrainingRepository trainingRepo,
    IOnlineTrainingSubscriptionRepository subscriptionRepo,
    IConfiguration configuration) : BaseApiController
{

    [HttpPost("create-checkout-session")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentIntentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string TraineeId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var onlineTraining = await trainingRepo.GetByIdAsync(request.OnlineTrainingId);
        if (onlineTraining == null)
            return NotFound("Online training not found");

        //Check for existing active subscription
        var hasActiveSubscription = await subscriptionRepo.HasActiveSubscriptionAsync(TraineeId, request.OnlineTrainingId);
        if (hasActiveSubscription)
            return BadRequest(new { message = "You already have an active subscription for this training." });

        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "EGP",
                        UnitAmount = (long?)onlineTraining.Price * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = onlineTraining.Title ?? "Online Training"
                        }
                    },
                    Quantity = 1,
                }
            ],
            Mode = "payment",
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "traineeId", TraineeId },
                    { "onlineTrainingId", request.OnlineTrainingId.ToString() }
                }
            },
            SuccessUrl = "https://your-frontend.com/success",
            CancelUrl = "https://your-frontend.com/cancel"
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return Ok(new { url = session.Url });
    }

    public class CreatePaymentIntentRequest
    {
        [Required]
        public int OnlineTrainingId { get; set; }
    }
}
