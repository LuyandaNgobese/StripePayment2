using Stripe;
using Microsoft.AspNetCore.Http;
using System.IO;
using StripePayment2.Models;//add


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Specific route for payment controller
app.MapControllerRoute(
    name: "payment",
    pattern: "Payment/{action=Index}",
    defaults: new { controller = "Payment" });

app.Run();

/* --- Stripe Webhook Endpoint ---
app.MapPost("/api/stripe/webhook", async (HttpRequest request, IConfiguration config) =>
{
    // Read the JSON payload from Stripe
    var json = await new StreamReader(request.Body).ReadToEndAsync();

    // Get the Stripe-Signature header
    var stripeSignature = request.Headers["Stripe-Signature"];

    // Get the webhook secret from appsettings.json
    var webhookSecret = config["Stripe:WebhookSecret"];

    try
    {
        // Construct the event and verify signature
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            stripeSignature,
            webhookSecret
        );

        // Handle the checkout session completed event
        if (stripeEvent.Type == "checkout.session.completed")

        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

            // TODO: mark the order as paid in your database
            Console.WriteLine($"Payment succeeded for session: {session.Id}");
        }

        return Results.Ok(); // Respond 200 OK to Stripe
    }
    catch (StripeException e)
    {
        // Respond 400 Bad Request if verification fails
        return Results.BadRequest(e.Message);
    }
});*/

// Run the app
app.Run();

// Stripe settings class
public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}