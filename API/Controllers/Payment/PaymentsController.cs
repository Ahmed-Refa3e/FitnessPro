using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Payment;

public class PaymentsController(SignInManager<ApplicationUser> signInManager,IOnlineTrainingRepository repo) : BaseApiController
{
    [HttpPost("create-payment-intent")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized("User not found");
        var onlineTraining = await repo.GetByIdAsync(request.OnlineTrainingId);
        if (onlineTraining == null)
            return NotFound("Online training not found");
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long?)onlineTraining.Price, // Amount in cents (2000 = $20.00) 
            PaymentMethodTypes = ["card"],
            Currency = "usd",
            Metadata = new Dictionary<string, string>
        {
            { "traineeId", user.Id },
            { "onlineTrainingId", request.OnlineTrainingId.ToString()}
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
    public int OnlineTrainingId { get; set; }
}


