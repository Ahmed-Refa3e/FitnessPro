using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers.Payment;

public class StripeWebhookController(
    IConfiguration configuration,
    ILogger<StripeWebhookController> logger,
    IOnlineTrainingSubscriptionRepository repo
) : BaseApiController
{
    private readonly string _webhookSecret = configuration["Stripe:WebhookSecret"]
        ?? throw new ArgumentNullException("Stripe Webhook Secret not configured.");

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];

        if (string.IsNullOrEmpty(stripeSignature))
        {
            logger.LogWarning("⚠️ Stripe signature header missing.");
            return Unauthorized();
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret, throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            logger.LogError($"🚨 Stripe signature verification failed: {ex.Message}");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError($"🚨 Unexpected error parsing webhook event: {ex.Message}");
            return BadRequest(new { error = "Invalid webhook payload" });
        }

        try
        {
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        logger.LogInformation($"✅ Payment succeeded for ID: {paymentIntent.Id}, Amount: {paymentIntent.Amount / 100.0} {paymentIntent.Currency.ToUpper()}");
                        await HandleSuccessfulPayment(paymentIntent);
                    }
                    break;

                case "payment_intent.payment_failed":
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    logger.LogWarning($"⚠️ Payment failed for ID: {failedIntent?.Id}");
                    break;

                default:
                    logger.LogInformation($"ℹ️ Unhandled event type: {stripeEvent.Type}");
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError($"❌ Error processing event {stripeEvent.Type}: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error while processing webhook" });
        }
    }

    private async Task HandleSuccessfulPayment(PaymentIntent paymentIntent)
    {
        try
        {
            if (!paymentIntent.Metadata.ContainsKey("traineeId") || !paymentIntent.Metadata.ContainsKey("onlineTrainingId"))
            {
                logger.LogWarning("❗ Missing metadata in payment intent.");
                return;
            }

            var traineeId = paymentIntent.Metadata["traineeId"];

            if (!int.TryParse(paymentIntent.Metadata["onlineTrainingId"], out var onlineTrainingId))
            {
                logger.LogWarning("❗ Invalid onlineTrainingId in metadata.");
                return;
            }

            var subscription = new OnlineTrainingSubscription
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                TraineeID = traineeId,
                OnlineTrainingId = onlineTrainingId,
                IsActive = true
            };

            repo.Add(subscription);
            await repo.SaveChangesAsync();

            logger.LogInformation($"✅ OnlineTrainingSubscription created for Trainee: {traineeId}");
        }
        catch (Exception ex)
        {
            logger.LogError($"❌ Failed to create OnlineTrainingSubscription: {ex.Message}");
            throw;
        }
    }
}
