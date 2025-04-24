using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Payment;

public class PaymentsController(SignInManager<ApplicationUser> signInManager, IOnlineTrainingRepository repo, IConfiguration configuration) : BaseApiController
{

    [HttpPost("create-checkout-session")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentIntentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized("User not found");

        var onlineTraining = await repo.GetByIdAsync(request.OnlineTrainingId);
        if (onlineTraining == null)
            return NotFound("Online training not found");

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
                    Currency = "usd",
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
                { "traineeId", user.Id },
                { "onlineTrainingId", request.OnlineTrainingId.ToString() }
            }
            },
            SuccessUrl = "https://your-frontend.com/success", // TODO: replace with your real success URL
            CancelUrl = "https://your-frontend.com/cancel"   // TODO: replace with your real cancel URL
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



