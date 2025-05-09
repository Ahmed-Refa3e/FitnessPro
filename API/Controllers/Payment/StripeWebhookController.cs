using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace API.Controllers.Payment;

public class StripeWebhookController(
    IConfiguration configuration,
    ILogger<StripeWebhookController> logger,
    IOnlineTrainingSubscriptionRepository repo) : BaseApiController
{
    private readonly string _webhookSecret = configuration["Stripe:WebhookSecret"] ??
        throw new ArgumentNullException("Stripe Webhook Secret not configured.");

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
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        logger.LogInformation($"✅ Checkout session completed. PaymentIntent: {session.PaymentIntent}, CustomerEmail: {session.CustomerEmail}");

                        var paymentIntentService = new PaymentIntentService();
                        var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

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

            // Check if already handled
            bool alreadyExists = await repo.PaymentIntentExistsAsync(paymentIntent.Id);
            if (alreadyExists)
            {
                logger.LogInformation($"ℹ️ PaymentIntent {paymentIntent.Id} already processed.");
                return;
            }

            var subscription = new OnlineTrainingSubscription
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                TraineeID = traineeId,
                OnlineTrainingId = onlineTrainingId,
                StripePaymentIntentId = paymentIntent.Id
            };

            // check if there is already a subscription for this trainee and online training
            var existingSubscriptions = await repo.GetByOnlineTrainingIdAsync(onlineTrainingId);
            if (existingSubscriptions != null && existingSubscriptions.Any(s => s!.TraineeID == traineeId))
            {
                logger.LogWarning($"⚠️ Subscription already exists for Trainee: {traineeId} and OnlineTraining: {onlineTrainingId}");
                return;
            }
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
