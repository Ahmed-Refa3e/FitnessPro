using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers.Payment;

public class StripeWebhookController(IConfiguration configuration, ILogger<StripeWebhookController> logger) : BaseApiController
{
    private readonly string _webhookSecret = configuration["Stripe:WebhookSecret"] ?? throw new ArgumentNullException("Stripe Webhook Secret not configured.");

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];

        if (string.IsNullOrEmpty(stripeSignature))
        {
            logger.LogWarning("⚠️ Stripe signature header missing.");
            return Unauthorized(); // 401 Unauthorized
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogError($"🚨 Stripe signature verification failed: {ex.Message}");
            return Unauthorized(); // 401 Unauthorized
        }
        catch (Exception ex)
        {
            logger.LogError($"🚨 Unexpected error parsing webhook event: {ex.Message}");
            return BadRequest(new { error = "Invalid webhook payload" }); // 400 Bad Request
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

    /// <summary>
    /// Marks order as paid in the database (Replace with actual DB logic).
    /// </summary>
    private async Task HandleSuccessfulPayment(PaymentIntent paymentIntent)
    {
        try
        {
            // Simulated database update (Replace with real database logic)
            logger.LogInformation($"📝 Marking order as paid for PaymentIntent ID: {paymentIntent.Id}");

            // Example: await _orderService.MarkOrderAsPaid(paymentIntent.Id);

            logger.LogInformation($"✅ Order successfully marked as paid for PaymentIntent ID: {paymentIntent.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError($"❌ Database update failed for PaymentIntent {paymentIntent.Id}: {ex.Message}");
            throw; // Ensures Stripe retries if the webhook fails
        }
    }
}
