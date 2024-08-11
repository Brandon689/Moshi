using Moshi.Ecommerce.Services;
using Stripe;

namespace Moshi.Ecommerce.Extensions;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/payments").WithTags("Payments"); ;

        group.MapPost("/create-payment-intent", CreatePaymentIntent)
            .WithName("CreatePaymentIntent")
            .WithOpenApi();

        group.MapPost("/webhook", HandleStripeWebhook)
            .WithName("StripeWebhook")
            .WithOpenApi();

        return endpoints;
    }

    private static IResult CreatePaymentIntent(PaymentIntentCreateRequest request, IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount,
            Currency = request.Currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", request.OrderId.ToString() },
            },
        };

        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);

        return Results.Ok(new { ClientSecret = paymentIntent.ClientSecret });
    }

    private static async Task<IResult> HandleStripeWebhook(HttpRequest request, IConfiguration config, OrderService orderService)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                request.Headers["Stripe-Signature"],
                config["Stripe:WebhookSecret"]
            );

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null && paymentIntent.Metadata.TryGetValue("OrderId", out var orderIdString))
                {
                    if (int.TryParse(orderIdString, out var orderId))
                    {
                        await orderService.UpdateOrderPaymentStatusAsync(orderId, "Paid");
                    }
                }
            }

            return Results.Ok();
        }
        catch (StripeException)
        {
            return Results.BadRequest();
        }
    }
}

public class PaymentIntentCreateRequest
{
    public int OrderId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; } = "usd";
}
